using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.Register;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ResponseModel> RegisterAsync(RegisterDTO dto);
        public Task<ResponseModel> LogInAsync(LogInDTO model);
        public Task<ResponseModel> VerifyOTP(string email, string otpCode);
        public Task<ResponseModel> ForgetPassword(string email);
        public Task<ResponseModel> ChangePassword(ChangePasswordDTO dto);
        public Task<ResponseModel> NewRefreshToken(string token);
        public Task<ResponseModel> LogOutAsync(string userId);
	}
}
