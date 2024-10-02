using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
