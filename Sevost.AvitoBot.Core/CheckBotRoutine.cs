using Microsoft.Extensions.Logging;
using Sevost.AvitoBot.Core.Ad;
using Sevost.AvitoBot.Core.HttpRequest;
using Sevost.AvitoBot.Core.TGBot;
using Sevost.AvitoBot.Database;
using Sevost.AvitoBot.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sevost.AvitoBot.Core
{
    public class CheckRoutine
    {
        private readonly ILogger _logger;
        private readonly AvitoPage _avitoPage;
        private readonly IAvitoPageParser _avitoPageParser;
        private readonly Telegram _telegram;
        private readonly Queue<string> _checkedAds;
        private readonly User _user;
        private readonly BotDbContext _dbContext;
        private readonly Dictionary<string, List<AdInfo>> _adsFromThisUri;

        public CheckRoutine(ILogger<CheckRoutine> logger, string token, BotDbContext dbContext, User user, CancellationTokenSource cancellationToken)
        {
            _avitoPage = new AvitoPage(logger);
            _avitoPageParser = new AvitoPageParser(logger);
            _telegram = new Telegram(logger, token);
            CancellationTokenSourse = cancellationToken;
            _checkedAds = new();
            _dbContext = dbContext;
            _user = user;
        }

        public CancellationTokenSource CancellationTokenSourse { get; private set; }

        public async void StartChecking()
        {
            while (!CancellationTokenSourse.Token.IsCancellationRequested)
            {
                var requests = _dbContext.Requests.ToList().Where(x => x.OwnerId == _user.Id && x.IsActive);
                var requestLinkHtmlResPair = new Dictionary<string, List<AdInfo>>();
                foreach (var request in requests)
                {
                    if (!requestLinkHtmlResPair.TryGetValue(request.Uri.OriginalString, out var ads))
                    {
                        await Task.Delay(6000);
                        var html = await _avitoPage.GetPageStringAsync(request.Uri, CancellationTokenSourse.Token).ConfigureAwait(false);
                        if (string.IsNullOrEmpty(html))
                            continue;

                        var adsString = _avitoPageParser.GetAdsBody(html);
                        var parsedAds = new List<AdInfo>();
                        foreach (var ad in adsString)
                        {
                            var link = _avitoPageParser.GetAdLink(ad, request.URIPattern);
                            if (_checkedAds.Contains(link))
                                continue;

                            parsedAds.Add(new AdInfo
                            {
                                Title = _avitoPageParser.GetAddTitle(ad),
                                Price = _avitoPageParser.GetAddPrice(ad),
                                Link = link
                            });
                        }

                        ads = parsedAds;
                    }

                    foreach (var ad in ads)
                    {
                        if (ad.Price <= request.PriceThreshold && ad.Title.Contains(request.SearchRequest))
                            await _telegram.SendMessage("", request.Owner.TGId);

                        _checkedAds.Enqueue(ad.Link);
                    }
                }
            }
        }

    }
}