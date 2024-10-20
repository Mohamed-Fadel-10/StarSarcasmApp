using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {

        private readonly IChatMessageService _chatMessageService;

        public ChatMessageController(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        [HttpGet("GetMessagesForChat")]
        public async Task<IActionResult> GetMessagesForChat(string chatId)
        {
            var response = await _chatMessageService.GetMessagesForChat(chatId);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
