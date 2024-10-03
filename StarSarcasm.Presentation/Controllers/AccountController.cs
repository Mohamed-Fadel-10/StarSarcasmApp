using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.ISMSService;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOTPService _oTPService;
       public AccountController(IAuthService _authService, IOTPService _oTPService)
        {
           this._authService = _authService;
            this._oTPService = _oTPService; 

        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInDTO model)
        { 
                var Response = await _authService.LogInAsync(model);
                if (Response.IsSuccess)
                {
                    return StatusCode(Response.StatusCode,Response.Message);
                }
           return StatusCode(Response.StatusCode,Response.Message);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(string phoneNumber, string otpCode)
        {
            var Response = await _authService.VerifyOTP(phoneNumber, otpCode);
            if (Response.IsSuccess)
            {
                return StatusCode(Response.StatusCode, new
                {
                    Message = Response.Message,
                    Token = Response.Model
                });
            }
            return StatusCode(Response.StatusCode, 
                new {
                    Message =Response.Message,
                    Token = Response.Model
                });
        }
    }
}
