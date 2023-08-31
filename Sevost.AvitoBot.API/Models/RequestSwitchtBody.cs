using Sevost.AvitoBot.API.Enums;

namespace Sevost.AvitoBot.API.Models
{
    public class RequestSwitchtBody
    {
        public Guid RequestId { get; set; }

        public long UserTgId { get; set; }

        // 0 - disable request
        // 1 - enable request
        public RequestSwitch OperationCode { get; set; }
    }
}