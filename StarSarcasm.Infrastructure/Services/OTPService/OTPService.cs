using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.OTP;
using StarSarcasm.Infrastructure.Data;
using System.Security.Cryptography;


namespace StarSarcasm.Infrastructure.Services.SMSServices
{
    public class OTPService : IOTPService
    {
        private readonly Context _context;
        public OTPService(Context context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
        }
        public async Task<string> GenerateOTP(string email)
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 999999);

            var expirationTime = DateTime.UtcNow.AddMinutes(5);

            var otpCode = new OTP
            {
                Email = email,
                Code = otp.ToString(),
                ExpirationTime = expirationTime
            };

            await _context.OTP.AddAsync(otpCode);
            await _context.SaveChangesAsync();

            return otp.ToString();

        }
      
    }
}
