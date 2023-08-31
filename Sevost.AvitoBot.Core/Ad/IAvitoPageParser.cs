using System.Collections.Generic;

namespace Sevost.AvitoBot.Core.Ad
{
    public interface IAvitoPageParser
    {
        IEnumerable<string> GetAdsBody(string htmlString);

        string GetAdLink(string input, string locationAndCategoryLinkPart);

        int GetAddPrice(string input);

        string GetAddTitle(string input);
    }
}