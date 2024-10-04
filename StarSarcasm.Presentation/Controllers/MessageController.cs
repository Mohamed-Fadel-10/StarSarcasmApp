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

        [HttpGet]
        public async Task<IActionResult> UserMessages(string id)
        {
            var Response=await _messageService.GetUserMessages(id);
            if (Response.Count>0) {
                return Ok(Response);
            }
            return BadRequest(Response);
        }
    }
}
