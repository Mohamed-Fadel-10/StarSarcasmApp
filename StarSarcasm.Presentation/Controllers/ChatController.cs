using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IWebHostEnvironment _env;

        public ChatController(IChatService chatService, IWebHostEnvironment env) { 
        _chatService = chatService;
            _env = env;
        }


        [HttpGet("GetUserChats")]
        public async Task<IActionResult> GetUserChats(string userId)
        {
            var response=await _chatService.GetUserChats(userId);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }



    }
}
