using AvitoTelegramBot.MusicInstrument;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AvitoMusicInstrumentsBot.MusicInstrument
{
    public class PriceData
    {
        public PriceData()
        {
            GetModelsDict();
            GetInsTypesDict();
        }

        public Dictionary<MusicInstrumentBrand, Brand> ModelsDictionary { get; set; }

        public Dictionary<MusicInstrumentType, int> InstrumentTypesDictionary { get; set; }

        private void GetModelsDict()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ModelsDictionary = new()
            {
                { MusicInstrumentBrand.Ashotone, JsonConvert.DeserializeObject<Brand>(File.ReadAllText(Path.Combine(path, "Ashtone.json"))) },
                { MusicInstrumentBrand.CheapShit, JsonConvert.DeserializeObject<Brand>(File.ReadAllText(Path.Combine(path, "CheapShit.json"))) },
                { MusicInstrumentBrand.Cort, JsonConvert.DeserializeObject<Brand>(File.ReadAllText(Path.Combine(path, "Cort.json"))) },
                { MusicInstrumentBrand.Cremona, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Cremona.json"))) },
                { MusicInstrumentBrand.Flight, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Flight.json"))) },
                { MusicInstrumentBrand.Hohner, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Hohner.json"))) },
                { MusicInstrumentBrand.Ibanez, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Ibanez.json"))) },
                { MusicInstrumentBrand.Squier, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Squier.json"))) },
                { MusicInstrumentBrand.Yamaha, JsonConvert.DeserializeObject < Brand >(File.ReadAllText(Path.Combine(path, "Yamaha.json"))) }
            };
        }

        private void GetInsTypesDict()
        {
            InstrumentTypesDictionary = new()
            {
                { MusicInstrumentType.Folk, 15000 },
                { MusicInstrumentType.Acoustic, 5000 },
                { MusicInstrumentType.Electric, 5000 },
                { MusicInstrumentType.Bass, 8000 },
                { MusicInstrumentType.Classic, 10000 },
                { MusicInstrumentType.IDC, 5000 }
            };
        }
    }
}
