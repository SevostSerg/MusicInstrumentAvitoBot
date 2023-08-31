using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sevost.AvitoBot.Core.Ad
{
    public class AvitoPageParser : IAvitoPageParser
    {
        private const string _pattern = "<div data-marker=\"item\" data-item-id=";
        private readonly ILogger _logger;
        private const int _numberOfAdsToTake = 8;
        private const string ItemPropPattern = "itemProp=\"url\" href=\"";
        private const string AvitoURL = "https://www.avito.ru";
        private const string TitlePattern = "title=\"Объявление «";
        private const string PricePattern = "itemProp=\"price\" content=\"";

        public AvitoPageParser(ILogger<CheckRoutine> logger)
        {
            _logger = logger;
        }

        public int GetAddPrice(string input)
        {
            var items = Regex.Split(input, PricePattern);
            // item example: "<meta itemProp="price" content="3999"/><meta itemProp="availability" content=..."
            // in input is only one "itemProp="price" content=" thing. So It splits into 2 items constantly while this input format
            var price = items.Last().Split('"').First();
            if (int.TryParse(price, out int result))
                return result;
            //If price = "Бесплатно" or "Не указана"
            return default;
        }

        public string GetAddTitle(string input)
        {
            var items = Regex.Split(input, TitlePattern);
            // item example: "target="_blank" title="Объявление «Предусилитель-темброблок с эффектами Cherub GT-6» 6 фотографий" rel="no..."
            // in input is only one "title="Объявление " thing. So It splits into 2 items constantly while this input format
            return items.Last().Split('»').First();
        }

        public string GetAdLink(string input, string locationAndCategoryLinkPart)
        {
            var items = Regex.Split(input, ItemPropPattern);
            // item example: ""/moskva/muzykalnye_instrumenty/predusilitel-tembroblok_s_effektami_cherub_gt-6_1062103951" target="_blank" title="Объявление «Предусилитель-т..."
            var refString = DetectRefString(items, locationAndCategoryLinkPart);
            if (!refString.Equals(String.Empty))
                return AvitoURL + refString.Split('"').First();

            return String.Empty;  //todo: redo 
        }

        /// <param name="items"></param>
        /// <param name="locationAndCategoryLinkPart">ex:/moskva/muzykalnye_instrumenty/</param>
        private string DetectRefString(IEnumerable<string> items, string locationAndCategoryLinkPart)
        {
            foreach (var splitedItem in items)
            {
                if (splitedItem.Contains(locationAndCategoryLinkPart))
                   return splitedItem;
            }

            return String.Empty;
        }

        /// <returns>Html code of every ad</returns>
        public IEnumerable<string> GetAdsBody(string htmlString)
        {
            try
            {
                return Regex.Split(htmlString, _pattern).ToList().GetRange(1, _numberOfAdsToTake); // hmmm...
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                return Array.Empty<string>();
            }
        }
    }
}
