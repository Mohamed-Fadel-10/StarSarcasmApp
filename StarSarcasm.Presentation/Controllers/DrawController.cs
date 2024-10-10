using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrawController : ControllerBase
    {
        private readonly IAwardDrawService _awardDrawService;

        public DrawController(IAwardDrawService awardDrawService)
        {
            _awardDrawService = awardDrawService;
        }

        [HttpGet("activeDraw")]
        public async Task<IActionResult> GetActiveDraw()
        {
            var response = await _awardDrawService.GetActiveDrawAsync();
            if(response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost("addDraw")]
        public async Task<IActionResult> Add([FromForm]DrawDTO dto)
        {
            var response = await _awardDrawService.AddAsync(dto);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

    }
}
