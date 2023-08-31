using System.Text.Json.Serialization;

namespace Sevost.AvitoBot.Database.Models
{
    public class Request
    {
        public Guid Id { get; set; }

        public Uri Uri { get; set; }

        public string URIPattern { get; set; }

        public string SearchRequest { get; set; }

        public int PriceThreshold { get; set; }

        public Guid OwnerId { get; set; }

        public bool IsActive { get; set; }

        [JsonIgnore]
        public User Owner { get; set; }
    }
}