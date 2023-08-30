using AvitoMusicInstrumentsBot.InstrumentAnalyzers;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.TGBot
{
    public class Telegram
    {
        private readonly string _token;
        private readonly ILogger _logger;
        private readonly string[] _chatIDs;
        private readonly HttpClient _client;

        public Telegram(ILogger<Bot> logger)
        {
            _client = new HttpClient();
            _chatIDs = File.ReadAllLines(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ChatIDs.txt"));
            _token = "2093972716:AAGR_aRecyB_5YgSQ-R3b6XtlvIJOJPbpio";
            _logger = logger;
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
