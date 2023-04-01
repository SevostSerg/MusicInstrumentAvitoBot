using Newtonsoft.Json;

namespace AvitoTelegramBot.MusicInstrument
{
    public class Brand
    {
        [JsonProperty("DefaultPrice")]
        public long DefaultPrice { get; set; }

        [JsonProperty("ElectricModels")]
        public Models ElectricModels { get; set; }

        [JsonProperty("BassModels")]
        public Models BassModels { get; set; }

        [JsonProperty("UkuleleModels")]
        public Models UkuleleModels { get; set; }

        [JsonProperty("AcousticModels")]
        public Models AcousticModels { get; set; }

        [JsonProperty("ClassicModels")]
        public Models ClassicModels { get; set; }
    }

    public partial class Models
    {
        [JsonProperty("Item1")]
        public string[] Titles { get; set; }

        [JsonProperty("Item2")]
        public long MaxPrice { get; set; }
    }
}