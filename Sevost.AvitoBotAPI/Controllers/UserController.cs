using AvitoBot.Database;
using Microsoft.AspNetCore.Mvc;
using Sevost.AvitoBotAPI.Models;

namespace Sevost.AvitoBotAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly BotDbContext _botDbContext;

        public UserController(BotDbContext botDbContext, ILogger<UserController> logger)
        {
            _botDbContext = botDbContext;  // temp for test
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult Get(AddRequestBody body)
        {

        }
    }
}