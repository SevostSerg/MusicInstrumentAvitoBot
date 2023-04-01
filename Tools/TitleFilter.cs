using AvitoMusicInstrumentsBot.Ad;
using System.Configuration;
using System.IO;

namespace AvitoMusicInstrumentsBot.Tools
{
    public class TitleFilter
    {
        private string[] _unwantedInfo;

        public TitleFilter()
        {
            _unwantedInfo = File.ReadAllLines(ConfigurationManager.AppSettings["unwantedInfo"]);
        }

        public bool IsThisAdTitleGood(AdInfo ad) => !DoesTitleContainUnwantedInfo(ad.Title);

        private bool DoesTitleContainUnwantedInfo(string title)
        {
            for (int i = 0; i < _unwantedInfo.Length; i++)
                if (title.Contains(_unwantedInfo[i]))
                    return true;

            return false;
        }
    }
}
