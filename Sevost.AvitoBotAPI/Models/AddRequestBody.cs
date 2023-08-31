namespace Sevost.AvitoBotAPI.Models
{
    public class AddRequestBody
    {
        public string SearchRequest { get; set; }

        public Uri Uri { get; set; }

        public int PriceThreshold { get; set; }

        public long UserTgId { get; set; }
    }
}
