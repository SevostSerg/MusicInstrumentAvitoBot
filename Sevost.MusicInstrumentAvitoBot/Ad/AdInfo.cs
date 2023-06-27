using AvitoMusicInstrumentsBot.InstrumentAnalyzers;
using AvitoMusicInstrumentsBot.MusicInstrument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.Ad
{
    public class AdInfo
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public int Price { get; set; }

        public MusicInstrumentInfo MusicInstrument { get; set; }
    }
}
