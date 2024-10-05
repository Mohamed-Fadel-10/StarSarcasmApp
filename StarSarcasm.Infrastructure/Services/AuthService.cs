using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.Register;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.OTP;
using StarSarcasm.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace StarSarcasm.Infrastructure.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOTPService _oTPService;
        private readonly Context _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IOTPService _oTPService,
            Context _context, IConfiguration _configuration, IEmailService emailService)
        {
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._oTPService = _oTPService;
            this._context = _context;
            this._configuration = _configuration;
            _emailService = emailService;
        }

        public async Task<ResponseModel> RegisterAsync(RegisterDTO model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = "This user is already exists.",
                    StatusCode = 400
                };
            }

            var user = new ApplicationUser
            {
                UserName = model.Name,
                Name = model.Name,
                Email = model.Email,
                FcmToken=model.FcmToken,
            };

            var result = await _userManager.CreateAsync(user,model.Password);
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

            var otp = await _oTPService.GenerateOTP(model.Email);           
            await _emailService.SendOtpAsync(model.Email, otp);

            return new ResponseModel { IsSuccess = true, StatusCode = 200, Message = "OTP Sent Successfully, it’s valid for 10 minuets." };

        }

        public async Task<ResponseModel> LogInAsync(LogInDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user,model.Password))
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid email or password!",
                    StatusCode = 400
                };
            }

            var token = await GenerateJwtToken(user);
            var refreshToken = "";
            DateTime refreshTokenExpiration;

            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                refreshToken = activeToken.Token;
                refreshTokenExpiration = activeToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = CreateRefreshToken();
                refreshToken = RefreshToken.Token;
                refreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await _userManager.UpdateAsync(user);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return new ResponseModel
            {
                Message = "Loged in Successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Model = new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken=refreshToken,
                    RefreshTokenExpiration=refreshTokenExpiration,
                    Roles = userRoles,
                }
            };

        }

        public async Task<ResponseModel> VerifyOTP(string email, string otpCode)
        {
            var otpRecord = await _context.OTP.FirstOrDefaultAsync(o => o.Email == email && o.Code == otpCode);

            if (otpRecord == null)
            {
                return new ResponseModel { IsSuccess = false, Message = "Invalid OTP", StatusCode = 400 };
            }

            if (otpRecord.ExpirationTime < DateTime.UtcNow)
            {
                return new ResponseModel { Message = "OTP Has Expired", IsSuccess = false, StatusCode = 400 };
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ResponseModel { Message = "User Not found", IsSuccess = false, StatusCode = 400 };
            }

            var token = await GenerateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.EmailConfirmed = true;
            try
            {
                _context.OTP.Remove(otpRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = $"{ex.Message}", IsSuccess = false, StatusCode = 400 };
            }

            return new ResponseModel
            {
                Message = $"Congartulations {user.Name}, Verfication is done successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Model = new
                { 
                    Token= new JwtSecurityTokenHandler().WriteToken(token),
                    Roles= userRoles,
                }
            };
        }

        public async Task<ResponseModel> ForgetPassword(string email)
        {
            if(await _userManager.FindByEmailAsync(email)==null)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Invalid email!"
                };
            }

            var otp = await _oTPService.GenerateOTP(email);
            await _emailService.SendOtpAsync(email, otp);

            return new ResponseModel 
            { 
                IsSuccess = true,
                StatusCode = 200,
                Message = "OTP Sent Successfully, please verify your email within 10 minuets."
            };

        }

        public async Task<ResponseModel> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Invalid email!"
                };
            }

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Failed to remove old password!"
                };
            }

            var addResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addResult.Succeeded)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Failed to create new password!"
                };
            }

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Password changed successfully.",
            };
        }


        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, User.UserName),
                new Claim(ClaimTypes.NameIdentifier, User.Id),
                new Claim(JwtRegisteredClaimNames.Sub, User.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,User.Email),
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

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddHours(3),
                CreatedOn = DateTime.UtcNow
            };
        }



    }
}
