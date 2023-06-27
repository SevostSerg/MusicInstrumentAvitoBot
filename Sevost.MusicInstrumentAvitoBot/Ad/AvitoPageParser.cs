using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace AvitoMusicInstrumentsBot.Ad
{
    public class AvitoPageParser : IAvitoPageParser
    {
        private string _pattern = ConfigurationManager.AppSettings["mainSplitPattern"];
        private readonly ILogger _logger;
        private int _numberOfAdsToTake = Convert.ToInt32(ConfigurationManager.AppSettings["numberOfAddsToGet"]);
        private string _pattern0 = ConfigurationManager.AppSettings["processorPattern0"];
        private string _pattern1 = ConfigurationManager.AppSettings["processorPattern1"];
        private string _pattern2 = ConfigurationManager.AppSettings["processorPattern2"];
        private string _avitoLink = ConfigurationManager.AppSettings["avitoLink"];
        private string _titlePattern = ConfigurationManager.AppSettings["titlePattern"];
        private string _pricePattern = ConfigurationManager.AppSettings["pricePattern"];

        public AvitoPageParser()
        {
            _logger = Program.Logger.CreateLogger(nameof(AvitoPageParser));
        }

        public int GetAddPrice(string input)
        {
            var items = Regex.Split(input, _pricePattern);
            // item example: "<meta itemProp="price" content="3999"/><meta itemProp="availability" content=..."
            // in input is only one "itemProp="price" content=" thing. So It splits into 2 items constantly while this input format
            var price = items.Last().Split('"').First();
            if (Int32.TryParse(price, out int result))
                return result;
            //If price = "Бесплатно" or "Не указана"
            return default;
        }

        public string GetAddTitle(string input)
        {
            var items = Regex.Split(input, _titlePattern);
            // item example: "target="_blank" title="Объявление «Предусилитель-темброблок с эффектами Cherub GT-6» 6 фотографий" rel="no..."
            // in input is only one "title="Объявление " thing. So It splits into 2 items constantly while this input format
            return items.Last().Split('»').First();
        }

        public string GetAdLink(string input)
        {
            var items = Regex.Split(input, _pattern1);
            // item example: ""/moskva/muzykalnye_instrumenty/predusilitel-tembroblok_s_effektami_cherub_gt-6_1062103951" target="_blank" title="Объявление «Предусилитель-т..."
            var refString = DetectRefString(items);
            if (!refString.Equals(String.Empty))
                return _avitoLink + refString.Split('"').First();

            return String.Empty;  //todo: redo 
        }

        private string DetectRefString([NotNull]string[] items)
        {
            foreach (var splitedItem in items)
            {
                if (splitedItem.Contains(_pattern2))
                   return splitedItem;
            }

            return String.Empty;
        }

        /// <returns>Html code of every ad</returns>
        public string[] GetAdsBody(string htmlString)
        {
            try
            {
                var ads = Regex.Split(htmlString, _pattern).ToList().GetRange(1, _numberOfAdsToTake); // hmmm...
                return ads.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Array.Empty<string>();
            }
        }
    }
}
