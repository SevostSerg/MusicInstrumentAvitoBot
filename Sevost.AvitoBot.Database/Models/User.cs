using System.Text.Json.Serialization;

namespace Sevost.AvitoBot.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public bool IsRoutineActive { get; set; }

        public string Token { get; set; }

        public string Name { get; set; }

        public long TGId { get; set; }

        [JsonIgnore]
        public List<Request> Requests { get; set; }
    }
}
