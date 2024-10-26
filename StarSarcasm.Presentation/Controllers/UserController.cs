using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
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

       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("NearestUsersInZodiac")]
        public async Task<IActionResult> NearestUsersInZodiac(string userId,int zodiacNum)
        {
            var response = await _userService.NearestUsersInZodiac(userId,zodiacNum);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);

            }
            return StatusCode(response.StatusCode, response.Message);
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

		[HttpPut("updateUser")]
		public async Task<IActionResult> UpdateUser(string userId,[FromForm] ProfileDTO dto)
		{
			var response = await _userService.UpdateAsync(userId,dto);
			if (response.IsSuccess)
			{
				return StatusCode(response.StatusCode, response.Model);
			}
			return StatusCode(response.StatusCode, response.Message);

		}
	}
}
