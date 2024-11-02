using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Domain.Entities;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("allSubscribers")]
        public async Task<IActionResult> GetAllSubscribers(int drawId)
        {
            var response = await _awardDrawService.GetAllSubscribers(drawId);
            if (response.IsSuccess)
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

		//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("randomDrawWinner")]
        public async Task<IActionResult> RandomDrawWinner(int activeDrawId)
        {
            var response = await _awardDrawService.RandomDrawWinner(activeDrawId);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
			return StatusCode(response.StatusCode, response.Message);

		}

        [HttpPatch("updateDraw")]
        public async Task<IActionResult> Update([FromForm][Required] int darwId, [FromForm] UpdateDrawDTO dto)
        {
            var response=await _awardDrawService.UpdateAsync(darwId, dto);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

		[HttpDelete("deleteDraw")]
		public async Task<IActionResult> Delete(int darwId)
		{
			var response = await _awardDrawService.DeleteAsync(darwId);
			if (response.IsSuccess)
			{
				return StatusCode(response.StatusCode, response.Model);
			}
			return StatusCode(response.StatusCode, response.Message);
		}

        [HttpGet("GetLastFourDraws")]
        public async Task<IActionResult> GetLastFourDraws()
        {
            var response = await _awardDrawService.GetLastFourDraws();
            return response.IsSuccess ?
                StatusCode(response.StatusCode, response.Model) :
                StatusCode(response.StatusCode, response.Model);
        }

        [HttpGet("GetAllDraws")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _awardDrawService.GetAll();
            return response.IsSuccess ?
                StatusCode(response.StatusCode, response.Model) :
                StatusCode(response.StatusCode, response.Message);
        }
    }
}
