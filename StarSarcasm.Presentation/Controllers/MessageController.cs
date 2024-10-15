using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("usermessages")]
        public async Task<IActionResult> UserMessages(string userId)
        {
            var Response = await _messageService.GetUserMessages(userId);
            if (Response.Count > 0)
            {
                return Ok(Response);
            }
            return NotFound("No Messages Until Now For This User");
        }
        [HttpGet("GetMessagesForChat")]
        public async Task<IActionResult> GetMessagesForChat(int chatId)
        {
            var response = await _messageService.GetMessagesForChat(chatId);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
