namespace AvitoBot.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long TGId { get; set; }

        public List<Request> Requests { get; set; }
    }
}
