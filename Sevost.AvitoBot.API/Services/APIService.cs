using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Sevost.AvitoBot.API.Models;
using Sevost.AvitoBot.Core;
using Sevost.AvitoBot.Database;
using Sevost.AvitoBot.Database.Models;

namespace Sevost.AvitoBot.API.Services
{
    public class APIService
    {
        private const string AvitoURL = "https://www.avito.ru/";

        private readonly BotDbContext _botDbContext;
        private readonly AppConfig _appConfig;
        private readonly Dictionary<Guid, CheckRoutine> _routines;
        private readonly IDataProtectionProvider _provider;

        public APIService(BotDbContext botDbContext, AppConfig appConfig, IDataProtectionProvider provider)
        {
            _botDbContext= botDbContext ?? throw new ArgumentNullException(nameof(botDbContext));
            _routines = new Dictionary<Guid, CheckRoutine>();
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            StartAllRoutinesAsync();
        }

        public async Task AddNewUserAsync(long userTgId, string tgName, CancellationToken ct = default)
        {
            var newUser = await _botDbContext.Users.AddAsync(new User
            {
                Name = tgName,
                TGId = userTgId
            }, ct).ConfigureAwait(false);

            await _botDbContext.SaveChangesAsync(ct);

            var newUserRoutine = new CheckRoutine(null, _appConfig.BotToken, _botDbContext, newUser.Entity, new CancellationTokenSource());
            _routines[newUser.Entity.Id] = newUserRoutine;
            newUserRoutine.StartChecking();
        }

        public async Task<string> GenerateTokenAsync(long userTgId, CancellationToken ct = default)
        {
            var user = await _botDbContext.Users.FirstOrDefaultAsync(x => x.TGId == userTgId, ct).ConfigureAwait(false);
            if (user == null)
                throw new Exception($"User({userTgId}) doesn't exist!");

            var token = Guid.NewGuid().ToString();
            var protector = _provider.CreateProtector(_appConfig.TokenKey);
            user.Token = token;
            _botDbContext.Entry(user).CurrentValues.SetValues(user);
            await _botDbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            return token;
        }

        public async Task StartNewUserRoutineAsync(long userTgId, CancellationToken ct = default)
        {
            var user = await _botDbContext.Users.FirstOrDefaultAsync(x => x.TGId == userTgId, ct).ConfigureAwait(false);
            if (user == null)
                throw new Exception($"User({userTgId}) doesn't exist!");

            if (_routines.ContainsKey(user.Id))
                throw new Exception($"User({userTgId}) already has active routine.");

            var newRoutine = new CheckRoutine(null, _appConfig.BotToken, _botDbContext, user, new CancellationTokenSource());
            _routines[user.Id] = newRoutine;
            newRoutine.StartChecking();

            user.IsRoutineActive = true;
            _botDbContext.Entry(user).CurrentValues.SetValues(user);
            await _botDbContext.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task StopUserRoutineAsync(long userTgId, CancellationToken ct = default)
        {
            var user = await _botDbContext.Users.FirstOrDefaultAsync(x => x.TGId == userTgId, ct).ConfigureAwait(false);
            if (user == null)
                throw new Exception($"User({userTgId}) doesn't exist!");

            if (_routines.TryGetValue(user.Id, out var routine))
            {
                routine.CancellationTokenSourse.Cancel();
                _routines.Remove(user.Id);

                user.IsRoutineActive = false;
                _botDbContext.Entry(user).CurrentValues.SetValues(user);
                await _botDbContext.SaveChangesAsync(ct).ConfigureAwait(false);
                return;
            }


            throw new Exception($"User({userTgId} has no active routines!)");
        }

        public async Task<IEnumerable<Request>> GetUserRequestsAsync(long userTgId, CancellationToken ct = default)
        {
            var user = await _botDbContext.Users.FirstOrDefaultAsync(x => x.TGId == userTgId, ct).ConfigureAwait(false);
            if (user == null)
                throw new Exception($"User({userTgId}) doesn't exist!");

            return _botDbContext.Requests.Where(x => x.OwnerId == user.Id);
        }

        public async Task AddNewRequestAsync(AddRequestBody requestBody, CancellationToken ct = default)
        {
            var user = await _botDbContext.Users.FirstOrDefaultAsync(x => x.TGId == requestBody.UserTgId, ct).ConfigureAwait(false);
            if (user == null)
                throw new Exception($"User({requestBody.UserTgId}) doesn't exist!");

            var userRequestsCount = await _botDbContext.Requests.CountAsync(x => x.OwnerId == user.Id, ct).ConfigureAwait(false);
            if (userRequestsCount == _appConfig.RequestsPerUserLimit)
                throw new Exception($"You can't add >{_appConfig.RequestsPerUserLimit} requests!");

            await _botDbContext.Requests.AddAsync(new Request
            {
                SearchRequest = requestBody.SearchRequest,
                IsActive = true,
                OwnerId = user.Id,
                PriceThreshold = requestBody.PriceThreshold,
                Uri = requestBody.Uri,
                URIPattern = GetUriPatternFromUri(requestBody.Uri),
            }, ct).ConfigureAwait(false);

            await _botDbContext.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        private void StartAllRoutinesAsync()
        {
            var users = _botDbContext.Users.ToList();
            foreach (var user in users)
            {
                var routine = new CheckRoutine(null, _appConfig.BotToken, _botDbContext, user, new CancellationTokenSource());
                _routines[user.Id] = routine;
                routine.StartChecking();
            }
        }

        private static string GetUriPatternFromUri(Uri uri)
        {
            if (!uri.OriginalString.StartsWith(AvitoURL))
                throw new Exception("It's not Avito link!");

            var withoutUrl = uri.OriginalString.Split(AvitoURL)[1];
            var res = withoutUrl.Split('/');
            return string.Concat(res[0], '/', res[1]);
        }
    }
}
