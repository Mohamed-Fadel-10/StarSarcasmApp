using StarSarcasm.Application.DTOs.LogIn;
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
        public Task<ResponseModel> LogInAsync(LogInDTO model);
        public Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User); 
        public Task<ResponseModel> VerifyOTP(string phoneNumber, string otpCode);
    }
}
