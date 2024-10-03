using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.Register;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.OTP;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio.Jwt.AccessToken;
using Twilio.Types;

namespace StarSarcasm.Infrastructure.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOTPService _oTPService;
        private readonly Context _context;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IOTPService _oTPService,
            Context _context, IConfiguration _configuration)
        {
            this._userManager= _userManager;
            this._roleManager= _roleManager;
            this._oTPService= _oTPService;
            this._context= _context;
            this._configuration= _configuration;
        }
        public async Task<ResponseModel> LogInAsync(LogInDTO model)
        {
            var isUserExist = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            if (isUserExist == null)
            {
                var user = new ApplicationUser
                {
                    UserName = model.PhoneNumber,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "Cannot Create User" };
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "Cannot Create Role" };
                    }
                }
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "Cannot Add User to Role" };
                }
            }

            var otp = _oTPService.GenerateOTP();
            var expirationTime = DateTime.UtcNow.AddMinutes(5);

            var otpCode = new OTP
            {
                PhoneNumber = model.PhoneNumber,
                Code = otp,
                ExpirationTime = expirationTime
            };

            await _context.OTP.AddAsync(otpCode);
            await _context.SaveChangesAsync();

            _oTPService.Send(model.PhoneNumber, otp);

            return new ResponseModel { IsSuccess = true, StatusCode = 200, Message = "OTP Sent Successfully" };
        }

        public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User)
        {
            var claims = new List<Claim>
                      {
                     new Claim(ClaimTypes.Name, User.UserName),
                     new Claim(ClaimTypes.NameIdentifier, User.Id),
                     new Claim(JwtRegisteredClaimNames.Sub, User.PhoneNumber),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim("uid", User.Id)
                     };
            var roles = await _userManager.GetRolesAsync(User);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SecurityKey Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            SigningCredentials signingCred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                claims: claims,
                signingCredentials: signingCred,
                expires: DateTime.UtcNow.AddHours(3)
                );
            return Token;
        }
        public async Task<ResponseModel> VerifyOTP(string phoneNumber, string otpCode)
        {
            var otpRecord = await _context.OTP.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber && o.Code == otpCode);

            if (otpRecord == null)
            {
                return new ResponseModel { IsSuccess = false, Message = "Invalid OTP", StatusCode = 400 };
            }

            if (otpRecord.ExpirationTime < DateTime.UtcNow)
            {
                return new ResponseModel { Message = "OTP Has Expired", IsSuccess = false, StatusCode = 400 };
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return new ResponseModel { Message = "User Not found", IsSuccess = false, StatusCode = 400 };
            }

            var token = await GenerateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.PhoneNumberConfirmed = true;
            try
            {
                _context.OTP.Remove(otpRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return new ResponseModel { Message = $"{ex.Message}", IsSuccess = false, StatusCode = 400 };
            }

            return new ResponseModel
            {
                Message = "Login Successfully",
                IsSuccess = true,
                StatusCode = 200,
                Model = new { 
                    Token= new JwtSecurityTokenHandler().WriteToken(token),
                    Roles= userRoles,
                }
            };
        }



    }
}
