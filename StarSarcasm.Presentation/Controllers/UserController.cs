using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var response=await _userService.GetAll();
            if (response == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, response);

            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet("GetAllInZodiac")]
        public async Task<IActionResult> GetAllInZodiac(int zodiacNum)
        {
            var response = await _userService.UsersWithZodiac(zodiacNum);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode,response.Model);

            }
            return StatusCode(response.StatusCode,response.Message);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile(string id)
        {
            var response = await _userService.Profile(id);
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Message);

            }
            return StatusCode(response.StatusCode, response.Model);
        }

        [HttpDelete]
        public async Task<IActionResult> ReomveUser(string id)
        {
            var response=await _userService.RemoveUser(id);
            if (response.IsSuccess)
            {
               return StatusCode(response.StatusCode,response.Message);
            }
            return StatusCode(response.StatusCode, response.Message);

        }
    }
}
