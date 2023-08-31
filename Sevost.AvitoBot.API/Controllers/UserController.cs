using Microsoft.AspNetCore.Mvc;
using Sevost.AvitoBot.API.Models;
using Sevost.AvitoBot.API.Services;

namespace Sevost.AvitoBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly APIService _apiService;

        public UserController(APIService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<ActionResult> UserRequests(long userTgId)
        {
            try
            {
                return Ok(await _apiService.GetUserRequestsAsync(userTgId).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewUser(long userTgId, string name)
        {
            try
            {
                await _apiService.AddNewUserAsync(userTgId, name).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewRequest(AddRequestBody body)
        {
            try
            {
                await _apiService.AddNewRequestAsync(body).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> StopChecking(long userTgId)
        {
            try
            {
                await _apiService.StopUserRoutineAsync(userTgId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpPost]
        public async Task<ActionResult> StartChecking(long userTgId)
        {
            try
            {
                await _apiService.StartNewUserRoutineAsync(userTgId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}