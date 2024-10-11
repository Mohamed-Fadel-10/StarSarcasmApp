using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;

namespace StarSarcasm.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserDrawController : ControllerBase
	{
		private readonly IUserDrawService _userDrawService;

		public UserDrawController(IUserDrawService userDrawService)
		{
			_userDrawService = userDrawService;
		}

		[HttpPost("addUserDraw")]
		public async Task<IActionResult> AddUserDraw(int drawId,string userId)
		{
			var response = await _userDrawService.AddAsync(drawId, userId);
			if (response.IsSuccess)
			{
				return StatusCode(response.StatusCode,response.Message);
			}
			return StatusCode(response.StatusCode,response.Message);
		}
	}
}
