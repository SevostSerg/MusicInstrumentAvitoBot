using AvitoMusicInstrumentsBot.Ad;
using AvitoMusicInstrumentsBot.MusicInstrument;
using AvitoMusicInstrumentsBot.TGBot;
using AvitoMusicInstrumentsBot.Tools;
using AvitoTelegramBot.MusicInstrument;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.InstrumentAnalyzers
{
    public class PriceAnalyzer
    {
        private bool _sent;
        private readonly ILogger _logger;
        private readonly ILogger _sendedMessagesLogger;
        private readonly PriceData _globalDictionary;
        private readonly TitleFilter _titleFilter;
        private readonly Bot _bot;

        public PriceAnalyzer()
        {
            _logger = Program.Logger.CreateLogger(nameof(PriceAnalyzer));
            _sendedMessagesLogger = Program.SendedMessagesLogger.CreateLogger(nameof(PriceAnalyzer));
            _titleFilter = new TitleFilter();
            _globalDictionary = new PriceData();
            _bot = new Bot();
        }

        public async Task CheckPrice(AdInfo adInfo)
        {
            if (adInfo.Price == 0)
                return;

            if (adInfo.MusicInstrument.Brand != MusicInstrumentBrand.Unknown)
                _sent = CheckPriceWithBrand(adInfo);
            else if (adInfo.MusicInstrument.Type != MusicInstrumentType.Unknown)
                _sent = CheckPriceWithType(adInfo);
            else
                return;

            await SendMessage(adInfo).ConfigureAwait(false);
        }

        public int AdsSended { get; private set; }

        private bool CheckPriceWithType(AdInfo adInfo)
        {
            if (!_globalDictionary.InstrumentTypesDictionary.TryGetValue(adInfo.MusicInstrument.Type, out var maxPrice))
                return false;

            return adInfo.Price < maxPrice;
        }

        private bool CheckPriceWithBrand(AdInfo adInfo)
        {
            if (!_globalDictionary.ModelsDictionary.TryGetValue(adInfo.MusicInstrument.Brand, out var brand))
                return false;

            return TryCheckPriceWithModel(adInfo, brand);
        }

        private bool TryCheckPriceWithModel(AdInfo adInfo, Brand brand)
        {
            var info = new Models[]
            {
                brand.AcousticModels,
                brand.BassModels,
                brand.UkuleleModels,
                brand.ClassicModels,
                brand.ElectricModels
            };

            foreach (var brandInfo in info)
                if (brandInfo.Titles != null)
                    foreach (var expectedTitlePart in brandInfo.Titles)
                        if (adInfo.Title.Contains(expectedTitlePart) && adInfo.Price <= brandInfo.MaxPrice)
                            return true;

            return adInfo.Price <= brand.DefaultPrice;
        }

        private async Task SendMessage(AdInfo adInfo)
        {
            if (_sent && _titleFilter.IsThisAdTitleGood(adInfo))
            {
                try
                {
                    var message = $"Title: {adInfo.Title}\nPrice: {adInfo.Price}\nLink: {adInfo.Link}";
                    await _bot.SendMessage(message).ConfigureAwait(false);
                    AvitoTelegramBot.Avito.AutoMessenger.SendAvitoMessage(adInfo.Link);
                    AdsSended++;
                    _sendedMessagesLogger.LogInformation(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}