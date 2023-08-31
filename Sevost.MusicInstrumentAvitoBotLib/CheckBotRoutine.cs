using AvitoBot.Database;
using AvitoBot.Database.Models;
using AvitoMusicInstrumentsBot.Ad;
using AvitoMusicInstrumentsBot.HttpRequest;
using AvitoMusicInstrumentsBot.TGBot;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot
{
    public class CheckBotRoutine
    {
        private readonly ILogger _logger;
        private readonly AvitoPage _avitoPage;
        private readonly IAvitoPageParser _avitoPageParser;
        private readonly Telegram _telegram;
        private readonly Queue<string> _checkedAds;
        private readonly User _user;
        private readonly BotDbContext _dbContext;

        public CheckBotRoutine(ILogger<CheckBotRoutine> logger, string token, BotDbContext dbContext, User user, CancellationToken cancellationToken)
        {
            _avitoPage = new AvitoPage(logger);
            _avitoPageParser = new AvitoPageParser(logger);
            _telegram = new Telegram(logger, token);
            CancellationToken = cancellationToken;
            _dbContext = dbContext;
        }

        public CancellationToken CancellationToken { get; private set; }

        public async Task StartCheckingAsync()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                var requests = _dbContext.Requests.Where(x => x.OwnerId == _user.Id);
                var requestLinkHtmlResPair = new Dictionary<string, List<(AdInfo Ad, Request Request)>>();
                foreach (var request in requests)
                {
                    if (!request.IsActive)
                        continue;

                    if (!requestLinkHtmlResPair.ContainsKey(request.Uri.OriginalString))
                    {
                        var html = await _avitoPage.GetPageStringAsync(request.Uri, CancellationToken).ConfigureAwait(false);
                        var ads = _avitoPageParser.GetAdsBody(html);
                        var parsedAds = new List<(AdInfo, Request)>(ads.Count());
                        foreach (var ad in ads)
                        {
                            var link = _avitoPageParser.GetAdLink(ad, request.URIPattern);
                            if (_checkedAds.Contains(link))
                                continue;

                            parsedAds.Add((new AdInfo
                            {
                                Title = _avitoPageParser.GetAddTitle(ad),
                                Price = _avitoPageParser.GetAddPrice(ad),
                                Link = link
                            }, request));
                            requestLinkHtmlResPair[request.Uri.OriginalString] = parsedAds;
                        }
                    }

                    var addRequestPairs = requestLinkHtmlResPair.SelectMany(x => x.Value);
                    foreach (var adRequestPair in addRequestPairs)
                    {
                        if (adRequestPair.Ad.Price <= adRequestPair.Request.PriceThreshold && adRequestPair.Ad.Title.Contains(adRequestPair.Request.SearchRequest))
                            await _telegram.SendMessage("", request.Owner.TGId);

                        _checkedAds.Enqueue(adRequestPair.Ad.Link);
                    }
                }
                

                await Task.Delay(6000);
            }
        }

    }
}