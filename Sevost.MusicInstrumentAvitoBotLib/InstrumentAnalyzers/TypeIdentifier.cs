using AvitoMusicInstrumentsBot.Ad;
using AvitoMusicInstrumentsBot.GuitarModels.Tools;
using AvitoMusicInstrumentsBot.MusicInstrument;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace AvitoMusicInstrumentsBot.InstrumentAnalyzers
{
    public class TypeIdentifier : Comparer
    {
        private string[] _bassKeywords;
        private string[] _classicKeywords;
        private string[] _elecricKeywords;
        private string[] _acousticKeywords;
        private string[] _folkKeywords;
        private string[] _iDontCareKeywords;

        public TypeIdentifier()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _bassKeywords = File.ReadAllLines(Path.Combine(path, "BassGuitarSamples.txt"));
            _classicKeywords = File.ReadAllLines(Path.Combine(path, "ClassicGuitarSamples.txt"));
            _elecricKeywords = File.ReadAllLines(Path.Combine(path, "ElectricGuitarSamples.txt"));
            _acousticKeywords = File.ReadAllLines(Path.Combine(path, "AcousticGuitarSamples.txt"));
            _folkKeywords = File.ReadAllLines(Path.Combine(path, "FolkSamples.txt"));
            _iDontCareKeywords = File.ReadAllLines(Path.Combine(path, "IDontCareSamples.txt"));
        }

        /// <summary>
        /// The order is important!
        /// </summary>
        /// <returns></returns>
        public MusicInstrumentType TryToDefineType(string title)
        {
            if (IsThisBass(title))
                return MusicInstrumentType.Bass;

            if (IsThisAcoustic(title))
                return MusicInstrumentType.Acoustic;

            if (IsThisClassic(title))
                return MusicInstrumentType.Classic;

            if (IsThisElectric(title))
                return MusicInstrumentType.Electric;

            if (IsThisFolk(title))
                return MusicInstrumentType.Folk;

            if (IsThisIDotCare(title))
                return MusicInstrumentType.IDC;

            return MusicInstrumentType.Unknown;
        }

        private bool IsThisBass(string title) => Compare(_bassKeywords, title);

        private bool IsThisAcoustic(string title) => Compare(_acousticKeywords, title);

        private bool IsThisClassic(string title) => Compare(_classicKeywords, title);

        private bool IsThisElectric(string title) => Compare(_elecricKeywords, title);

        private bool IsThisFolk(string title) => Compare(_folkKeywords, title);

        private bool IsThisIDotCare(string title)
        {
            foreach (var expectedTitle in _iDontCareKeywords)
                if (expectedTitle == title)
                    return true;

            return false;
        }
    }
}
