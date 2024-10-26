using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppMessageController : ControllerBase
    {
        private readonly IAppMessageService _appMessageService;

        public AppMessageController(IAppMessageService appMessageService)
        {
            _appMessageService = appMessageService;
        }

        [HttpPost("uploadMessagesfile")]
        public async Task<IActionResult> UploadMessagesFile(IFormFile file)
        {
            var response=await _appMessageService.UploadMessagesExcelFile(file);

            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost("addMessage")]
        public async Task<IActionResult> AddMessage(string msg)
        {
            var response = await _appMessageService.AddMessage(msg);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
