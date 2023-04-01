using AvitoMusicInstrumentsBot.GuitarModels.Tools;
using AvitoMusicInstrumentsBot.MusicInstrument;

namespace AvitoMusicInstrumentsBot.InstrumentAnalyzers
{
    public class BrandIdentifier : Comparer
    {
        private string[] _ibanezTitles      = { "Ibanez", "ibanez", "ибанез", "Ибанез", "IBanez", "IbaneZ" };
        private string[] _squierTitles      = { "Squier", "squier", "squer", "скваер", "Скваер" };
        private string[] _yamahaTitles      = { "Yamaha", "yamaha", "ямаха", "Ямаха" };
        private string[] _hohnerTitles      = { "Hohner", "hohner", "Honer", "honer", "Hofner", "Хохнер", "хохнер", "Хонер", "хонер" };
        private string[] _fenderTitles      = { "Fender", "fender", "Фердер", "фердер" };
        private string[] _epiphoneTitles    = { "Epiphone", "epiphone", "Эпифон", "эпифон" };
        private string[] _cremonaTitles     = { "Cremona", "cremona", "Kremona", "kremona", "Strunal", "Кремона", "кремона" };
        private string[] _deanTitles        = { "Dean", "dean" };
        private string[] _schecterTitles    = { "Schecter", "schecter", "Шектер", "шектер" };
        private string[] _martinezTitles    = { "Martinez", "martinez", "Martines", "martines", "Мартинез", "мартинез" };
        private string[] _gibsonTitles      = { "Gibson", "gibson", "Гибсон", "гибсон" };
        private string[] _crafterTitles     = { "Crafter", "crafter", "Крафтер", "крафтер" };
        private string[] _chineseShitTitles = { "Colombo", "DENN", "Denn", "Aria", "Adams", "Madeira", "colombo", "DENN", "denn", "aria", "adams" };
        private string[] _cortTitles        = { "Cort", "cort", "Корт", "корт" };
        private string[] _flightTitles      = { "Flight", "flight", "Флайт", "флайт" };
        private string[] _staggTitles       = { "Stagg", "stagg", "STAGG", "Стаг", "Стагг", "СТАГ", "СТАГГ" };
        private string[] _ashtoneTitles     = { "shtone", "SHTONE", "Эштон", "ЭШТОН" };

        public MusicInstrumentBrand TryToDefineBrand(string title)
        // The order is important (for example Fender and Fender Squier)
        {
            if (Compare(_ibanezTitles, title))
                return MusicInstrumentBrand.Ibanez;

            if (Compare(_squierTitles, title))
                return MusicInstrumentBrand.Squier;

            if (Compare(_yamahaTitles, title))
                return MusicInstrumentBrand.Yamaha;

            if (Compare(_hohnerTitles, title))
                return MusicInstrumentBrand.Hohner;

            if (Compare(_fenderTitles, title))
                return MusicInstrumentBrand.Fender;

            if (Compare(_epiphoneTitles, title))
                return MusicInstrumentBrand.Epiphone;

            if (Compare(_cremonaTitles, title))
                return MusicInstrumentBrand.Cremona;

            if (Compare(_deanTitles, title))
                return MusicInstrumentBrand.Dean;

            if (Compare(_schecterTitles, title))
                return MusicInstrumentBrand.Schecter;

            if (Compare(_martinezTitles, title))
                return MusicInstrumentBrand.Martinez;

            if (Compare(_gibsonTitles, title))
                return MusicInstrumentBrand.Gibson;

            if (Compare(_crafterTitles, title))
                return MusicInstrumentBrand.Crafter;

            if (Compare(_chineseShitTitles, title))
                return MusicInstrumentBrand.CheapShit;

            if (Compare(_cortTitles, title))
                return MusicInstrumentBrand.Cort;

            if (Compare(_flightTitles, title))
                return MusicInstrumentBrand.Flight;

            if (Compare(_staggTitles, title))
                return MusicInstrumentBrand.Stagg;

            if (Compare(_ashtoneTitles, title))
                return MusicInstrumentBrand.Ashotone;

            return MusicInstrumentBrand.Unknown;
        }
    }
}
