using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AwardDrawController : ControllerBase
    {
        private readonly IAwardDrawService _awardDrawService;

        public AwardDrawController(IAwardDrawService awardDrawService)
        {
            _awardDrawService = awardDrawService;
        }

        [HttpGet("awardDraw")]
        public async Task<IActionResult> GetActiveDraw()
        {
            var response = await _awardDrawService.GetActiveDrawAsync();
            if(response != null)
            {
                return StatusCode(StatusCodes.Status200OK, response);
            }
            return BadRequest();
        }

        [HttpPost("addDraw")]
        public async Task<IActionResult> Add(AwardDrawDTO dto)
        {
            var response=await _awardDrawService.AddAsync(dto);
            if (response != null)
            {
                return StatusCode(StatusCodes.Status200OK, response);
            }
            return BadRequest();
        }
    }
}
