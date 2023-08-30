using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvitoMusicInstrumentsBot.HttpRequest
{
    public class AvitoPage
    {
        private readonly string _url;
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private const int Timeout = 15000;

        public AvitoPage(ILogger<Bot> logger)
        {
            _url = "https://www.avito.ru/moskva/muzykalnye_instrumenty/gitary_i_strunnye-ASgBAgICAUTEAsYK?cd=1&s=104&user=1";
            _logger = logger;
            _client = new HttpClient();
        }

        public async Task<string> GetPageStringAsync()
        {
            using var response = await _client.GetAsync(_url).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}