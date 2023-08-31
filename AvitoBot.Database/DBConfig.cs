using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AvitoBot.Database
{
    public class DBConfig
    {
        private readonly IConfiguration _configuration;

        public DBConfig(IConfiguration configuration) 
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string ConnectionString
        {
            get
            {
                return new NpgsqlConnectionStringBuilder()
                {
                    Host = _configuration.GetSection("DBConfig:Host").Value,
                    Port = Convert.ToInt32(_configuration.GetSection("DBConfig:Port").Value),
                    Database = _configuration.GetSection("DBConfig:Database").Value,
                    Username = _configuration.GetSection("DBConfig:Username").Value,
                    Password = _configuration.GetSection("DBConfig:Password").Value
                }.ConnectionString;
            }
        }
    }
}