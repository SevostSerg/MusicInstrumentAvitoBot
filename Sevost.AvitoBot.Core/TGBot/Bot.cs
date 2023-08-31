using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sevost.AvitoBot.Core.TGBot
{
    public class Telegram
    {
        private readonly string _token;
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public Telegram(ILogger<CheckRoutine> logger, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            _token = token;
            _client = new HttpClient();
            _logger = logger;
        }

        public async Task SendMessage(string message, long tgId)
            => await _client.GetAsync($"https://api.telegram.org/bot{_token}/sendMessage?chat_id={tgId}&text={message}").ConfigureAwait(false);
    }
}
