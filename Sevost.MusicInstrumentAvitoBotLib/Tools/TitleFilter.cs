using AvitoMusicInstrumentsBot.Ad;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace AvitoMusicInstrumentsBot.Tools
{
    public class TitleFilter
    {
        private string[] _unwantedInfo;

        public TitleFilter()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _unwantedInfo = File.ReadAllLines(Path.Combine(path, ".botIgnore"));
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
