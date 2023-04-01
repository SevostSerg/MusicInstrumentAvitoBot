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
    public class Test
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

        public Test()
        {
            _startOfSession = DateTime.Now;
            _checkedAds = new();
            _avitoPageParser = new AvitoPageParser();
            _typeIdentifier = new TypeIdentifier();
            _brandIdentifier = new BrandIdentifier();
            _priceAnalyzer = new PriceAnalyzer();
            _avito = new AvitoPage();
            _loopTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["loopTimeout"]);
            _numberOfAdsAtOneCheck = Convert.ToInt32(ConfigurationManager.AppSettings["numberOfAddsToGet"]);
            _threshold = _numberOfAdsAtOneCheck + 15;
            _logger = Program.Logger.CreateLogger(nameof(Test));
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public async Task Start()
        {
            for (; ; )
            {
                _sessionTime = DateTime.Now - _startOfSession;
                _oldCount = _currCount;
                _numberOfLoop++;
                Console.Clear();
                Console.WriteLine($"Session: {Math.Round(_sessionTime.TotalHours, 2)} Hours" +
                    $" NumberOfCheck: {_numberOfLoop} | NumberOfMessages: " +
                    $"{_priceAnalyzer.AdsSended}\n" +
                    $"---------------------------------------------------------------");
                try
                {
                    _responce = await _avito.GetPageStringAsync().ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    var errorMessage = $"Error while sending request: {ex.StatusCode}";
                    Console.WriteLine(errorMessage);
                    _logger.LogError(errorMessage);
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

                        if (_currCount == _oldCount)
                            Console.WriteLine($"No new adds, adds checked: {_currCount} Add buffer count: {_checkedAds.Count}");

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
                if (_numberOfLoop % 10 == 0)
                    GC.Collect();
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
            Console.WriteLine($"Title: {_currentAnalyzedAd.Title}\n" +
                $"Type: {_currentAnalyzedAd.MusicInstrument.Type}\n" +
                $"Brand: {_currentAnalyzedAd.MusicInstrument.Brand}\n" +
                $"Price: {_currentAnalyzedAd.Price}\nLink: {_currentAnalyzedAd.Link}\n" +
                "---------------------------------------------------------------");
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
            Console.WriteLine($"Next at: {DateTime.Now.AddMicroseconds(_loopTimeout).TimeOfDay:hh\\:mm\\:ss}");
            Thread.Sleep(_loopTimeout);
        }
    }
}