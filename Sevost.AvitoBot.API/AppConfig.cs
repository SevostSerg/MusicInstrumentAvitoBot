namespace Sevost.AvitoBot.API
{
    public class AppConfig
    {
        private readonly IConfiguration _configuration;

        public AppConfig()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        private string _botToken;
        public string BotToken
        {
            get
            {
                _botToken ??= _configuration.GetValue<string>(nameof(BotToken));
                return _botToken;
            }
        }

        private string _tokenKey;
        public string TokenKey
        {
            get
            {
                _tokenKey ??= _configuration.GetValue<string>(nameof(TokenKey));
                return _tokenKey;
            }
        }

        private int _requestsPerUserLimit;
        public int RequestsPerUserLimit
        {
            get
            {
                if (_requestsPerUserLimit == default)
                    _requestsPerUserLimit = _configuration.GetValue<int>(nameof(RequestsPerUserLimit));

                return _requestsPerUserLimit;
            }
        }
    }
}