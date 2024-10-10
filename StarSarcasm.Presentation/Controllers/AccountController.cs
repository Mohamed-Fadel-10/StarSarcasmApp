using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.RefreshToken;
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
        private readonly IEmailService _emailService;

		public AccountController(IAuthService _authService, IOTPService _oTPService, IEmailService emailService)
		{
			this._authService = _authService;
			this._oTPService = _oTPService;
			_emailService = emailService;
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

        [HttpPost("RefreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshDTO dto)
        {
            if (ModelState.IsValid)
            {
                var Response = await _authService.NewRefreshToken(dto.RefreshToken);
                if (Response.IsSuccess )
                {
                    return StatusCode(Response.StatusCode,Response.Model);
                }
                return StatusCode(Response.StatusCode, Response.Model);
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
                return StatusCode(Response.StatusCode, Response.Message);
            }
            return StatusCode(Response.StatusCode, Response.Message);
        }

		[HttpPost("resendOtp")]
		public async Task<IActionResult> ResendOTP(string email)
		{
			var Response = await _emailService.ReSendOtpAsync(email);
			if (Response.IsSuccess)
			{
				return StatusCode(Response.StatusCode, Response.Message);
			}
			return StatusCode(Response.StatusCode, Response.Message);
		}
	}
}
