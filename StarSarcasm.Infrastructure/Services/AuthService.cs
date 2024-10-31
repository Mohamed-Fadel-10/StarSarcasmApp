using Microsoft.AspNetCore.Components.Routing;
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
            try
            {
                var isEmailExisted = await _userManager.FindByEmailAsync(model.Email);

                if (isEmailExisted != null)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "تم استخدام هذا الحساب من قبل",
                        StatusCode = 400
                    };
                }

                var user = new ApplicationUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    FcmToken = string.Empty,
                    Longitude = model.Longitude,
                    Latitude = model.Latitude,
                    BirthDate = model.BirthDate.Date,
                    UserName= model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "اسم المستخدم موجود من قبل " };
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "حدث خطأ , قم بالمحاولة مرة اخرى" };
                    }
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = $"{"User"} لا يمكن اضافة هذا المستخدم لهذه الصلاحية " };
                }

                var otp = await _oTPService.GenerateOTP(model.Email);
                await _emailService.SendOtpAsync(model.Email, otp);

                return new ResponseModel { IsSuccess = true, StatusCode = 200, Message = "تم ارسال الرقم التأكيدى الى بريدك الالكترونى قم بمراجعتة الرقم ومن ثم تأكيده" };
            }
            catch(Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ غير متوقع، يرجى المحاولة لاحقًا"
                };
            }
        }

        public async Task<ResponseModel> LogInAsync(LogInDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "خطأ فى كلمة السر او البريد الالكترونى",
                        StatusCode = 401
                    };
                }
                if (!user.EmailConfirmed)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "خطأ فى كلمة السر او البريد الالكترونى",
                        StatusCode = 403
                    };
                }

                if (model.FcmToken != user.FcmToken)
                {
                    user.FcmToken = model.FcmToken;
                    await _userManager.UpdateAsync(user);
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
                    Message = "تم تسجيل الخول بنجاح",
                    IsSuccess = true,
                    StatusCode = 200,
                    Model = new
                    {
                        UserId = user.Id,
                        IsSubscribed = user.IsSubscribed,
                        Name = user.Name,
                        Longitude = user.Longitude,
                        Latitude= user.Latitude,
                        BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = refreshTokenExpiration,
                        Roles = userRoles,
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ غير متوقع، يرجى المحاولة لاحقًا"
                };
            }
        }

        public async Task<ResponseModel> VerifyOTP(string email, string otpCode)
        {
            try
            {
                var otpRecord = await _context.OTP.FirstOrDefaultAsync(o => o.Email == email && o.Code == otpCode);

                if (otpRecord == null)
                {
                return new ResponseModel { IsSuccess = false, Message = "الرمز التأكيدى غير صالح", StatusCode = 400 };
                }

                if (otpRecord.ExpirationTime < DateTime.UtcNow)
                {
                return new ResponseModel { Message = "انتهت فترة السماحية للرمز التاكيدى ، قم بطلب لاعادة ارسال رمز اخر لبريدك الالكترونى", IsSuccess = false, StatusCode = 400 };
                }

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                return new ResponseModel { Message = "المستخدم غير موجود", IsSuccess = false, StatusCode = 400 };
                }

                user.EmailConfirmed = true;         
                _context.OTP.Remove(otpRecord);
                await _context.SaveChangesAsync();
           

                return new ResponseModel
                {
                    Message = $"تم تأكيد حسابك بنجاح قم بالتوجه لصفحة تسجيل الدخول الان",
                    IsSuccess = true,
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = $"{ex.Message}", IsSuccess = false, StatusCode = 400 };
            }
        }

        public async Task<ResponseModel> ForgetPassword(string email)
        {
            if(await _userManager.FindByEmailAsync(email)==null)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "البريد الالكترونى غير صالح"
                };
            }

            var otp = await _oTPService.GenerateOTP(email);
            await _emailService.SendOtpAsync(email, otp);

            return new ResponseModel 
            { 
                IsSuccess = true,
                StatusCode = 200,
                Message = "تم ارسال الرقم التأكيدى الى بريدك الالكترونى قم بمراجعتة الرقم ومن ثم تأكيده"
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
                    Message = "البريد الالكترونى غير صالح"
                };
            }

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = " حدث خطأ يرجى المحاولة مرة اخرى"
                };
            }

            var addResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addResult.Succeeded)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "حدث خطأ يرجى المحاولة مرة اخرى"
                };
            }

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "تم تغيير كلمة السر بنجاح",
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
                expires: DateTime.UtcNow.AddDays(1)
                );
            return Token;
        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber); 

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<ResponseModel> NewRefreshToken(string token)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
                if (user == null)
                {
                    return new ResponseModel { IsSuccess = false, Message = "Not Authenticated User", StatusCode = 400 };
                }

                var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

                if (!refreshToken.IsActive)
                {
                    return new ResponseModel
                    {
                        Message = "InActive Token",
                        StatusCode = 400,
                        Model = new
                        {
                            IsAuthenticated = false,
                            Name = string.Empty,
                            Roles = new List<string>(),
                            Token = string.Empty,
                            RefreshToken = string.Empty,
                            RefreshTokenExpiration = string.Empty
                        }
                    };
                }

                refreshToken.RevokedOn = DateTime.UtcNow;

                var newRefreshToken = CreateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
                var Roles = await _userManager.GetRolesAsync(user);
                var jwtToken = await GenerateJwtToken(user);

                return new ResponseModel
                {
                    Message = "Active Token",
                    StatusCode = 200,
                    Model = new
                    {
                        IsAuthenticated = true,
                        Name = user.Name,
                        Roles = Roles.ToList(),
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        RefreshToken = newRefreshToken.Token,
                        RefreshTokenExpiration = newRefreshToken.ExpiresOn
                    }
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ غير متوقع، يرجى المحاولة لاحقًا"
                };

            }
        }

    }
}
