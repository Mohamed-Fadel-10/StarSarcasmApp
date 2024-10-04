using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.Register;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.RegisterAsync(dto);
                if (response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
                return BadRequest(response.Message);
            }
            return BadRequest(ModelState);

        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInDTO model)
        { 
                var Response = await _authService.LogInAsync(model);
                if (Response.IsSuccess)
                {
                    return StatusCode(Response.StatusCode, Response.Model);
                }
           return StatusCode(Response.StatusCode,Response.Message);
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var response=await _authService.ForgetPassword(email);
                if (response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
                return BadRequest(response.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.ChangePassword(dto);
                if (response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
                return BadRequest(response.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("verifyOtp")]
        public async Task<IActionResult> VerifyOTP(string email, string otpCode)
        {
            var Response = await _authService.VerifyOTP(email, otpCode);
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
