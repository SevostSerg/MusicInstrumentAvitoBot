using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.HttpRequest
{
    public class AvitoPage
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public AvitoPage(ILogger<CheckBotRoutine> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        public async Task<string> GetPageStringAsync(Uri uri, CancellationToken ct)
        {
            using var response = await _client.GetAsync(uri, ct).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }
    }
}