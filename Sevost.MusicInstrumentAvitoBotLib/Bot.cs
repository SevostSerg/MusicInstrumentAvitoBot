using AvitoMusicInstrumentsBot.Ad;
using AvitoMusicInstrumentsBot.HttpRequest;
using AvitoMusicInstrumentsBot.InstrumentAnalyzers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot
{
    public class Bot
    {
        private const int ErrorTimeout = 15000;
        private string _responce;
        private string[] _currentAds;
        private readonly IAvitoPageParser _avitoPageParser;
        private readonly TypeIdentifier _typeIdentifier;
        private readonly BrandIdentifier _brandIdentifier;
        private readonly PriceAnalyzer _priceAnalyzer;
        private readonly AvitoPage _avito;
        private readonly ILogger _logger;
        private readonly int _loopTimeout;
        private readonly Queue<string> _checkedAds;
        private string _linkOfCurrAnalyzedAd;
        private AdInfo _currentAnalyzedAd;
        private int _currCount;
        private int _oldCount;
        private int _numberOfLoop;
        private readonly DateTime _startOfSession;
        private TimeSpan _sessionTime;
        private readonly int _numberOfAdsAtOneCheck;
        private readonly int _threshold;
        private DateTime _startOfLoop;

        public Bot(ILogger<Bot> logger)
        {
            _startOfSession = DateTime.Now;
            _checkedAds = new();
            _avitoPageParser = new AvitoPageParser(logger);
            _typeIdentifier = new TypeIdentifier();
            _brandIdentifier = new BrandIdentifier();
            _priceAnalyzer = new PriceAnalyzer(logger);
            _avito = new AvitoPage(logger);
            _loopTimeout = Convert.ToInt32(8000);
            _numberOfAdsAtOneCheck = Convert.ToInt32(5);
            _threshold = _numberOfAdsAtOneCheck + 15;
            _logger = logger;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public async Task StartAsync()
        {
            for (; ; )
            {
                _startOfLoop = DateTime.Now;
                _sessionTime = DateTime.Now - _startOfSession;
                _oldCount = _currCount;
                _numberOfLoop++;
                try
                {
                    _responce = await _avito.GetPageStringAsync().ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    var errorMessage = $"Error while sending request: {ex.StatusCode}";
                    _logger.LogError(errorMessage);
                    Thread.Sleep(ErrorTimeout);
                    continue;
                }
                catch
                {
                    Thread.Sleep(ErrorTimeout);
                    continue;
                }

                try
                {
                    if (_responce != string.Empty)
                    {
                        _currentAds = _avitoPageParser.GetAdsBody(_responce);
                        foreach (var adInHTML in _currentAds)
                            await AnalyzeRawAd(adInHTML).ConfigureAwait(false);

                        CleanCheckedList();
                    }
                }
                catch
                {
                    var errorMessage = "Error while parsing Avito page!";
                    _logger.LogError(errorMessage);
                    break;
                }


                Sleep();
            }
        }

        private async Task AnalyzeRawAd(string adInHTML)
        {
            _linkOfCurrAnalyzedAd = _avitoPageParser.GetAdLink(adInHTML);
            if (_checkedAds.Contains(_linkOfCurrAnalyzedAd))
                return;

            _currentAnalyzedAd = ParseAd(_avitoPageParser.GetAddTitle(adInHTML), _avitoPageParser.GetAddPrice(adInHTML), _linkOfCurrAnalyzedAd);
            await _priceAnalyzer.CheckPrice(_currentAnalyzedAd).ConfigureAwait(false);
            _checkedAds.Enqueue(_linkOfCurrAnalyzedAd);
            _currCount++;
        }

        private AdInfo ParseAd(string title, int price, string link)
        {
            return new AdInfo()
            {
                Title = title,
                Price = price,
                Link = link,
                MusicInstrument = new MusicInstrumentInfo()
                {
                    Brand = _brandIdentifier.TryToDefineBrand(title),
                    Type = _typeIdentifier.TryToDefineType(title)
                }
            };
        }

        private void CleanCheckedList()
        {
            if (_checkedAds.Count >= _threshold)
                while (_checkedAds.Count != _threshold)
                    _checkedAds.Dequeue();
        }

        private void Sleep()
        {
            var loopTime = DateTime.Now - _startOfLoop;
            Thread.Sleep(_loopTimeout);
        }
    }
}