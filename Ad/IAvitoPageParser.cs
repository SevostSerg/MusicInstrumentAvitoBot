using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.Ad
{
    public interface IAvitoPageParser
    {
        public string[] GetAdsBody(string input);

        public string GetAdLink(string input);

        public int GetAddPrice(string input);

        public string GetAddTitle(string input);
    }
}
