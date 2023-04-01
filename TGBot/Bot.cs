using AvitoMusicInstrumentsBot.InstrumentAnalyzers;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.TGBot
{
    public class Bot
    {
        private readonly string _token;
        private readonly ILogger _logger;
        private readonly string[] _chatIDs;
        private readonly HttpClient _client;

        public Bot()
        {
            _client = new HttpClient();
            _chatIDs = File.ReadAllLines(ConfigurationManager.AppSettings["chatIDs"]);
            _token = ConfigurationManager.AppSettings["token"];
            _logger = Program.Logger.CreateLogger(nameof(Bot));
        }

        public async Task SendMessage(string message)
        {
            foreach (var id in _chatIDs)
            {
                var url = $"https://api.telegram.org/bot{_token}/sendMessage?chat_id={id}&text={message}";
                await _client.GetAsync(url).ConfigureAwait(false);
            }
        }
    }
}
