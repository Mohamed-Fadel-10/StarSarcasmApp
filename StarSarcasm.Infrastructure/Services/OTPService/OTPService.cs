using Microsoft.AspNetCore.Identity;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System.Security.Cryptography;


namespace StarSarcasm.Infrastructure.Services.SMSServices
{
    public class OTPService : IOTPService
    {

        public OTPService(Context _context, UserManager<ApplicationUser> _userManager)
        {
        }
        public string GenerateOTP()
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 999999);

            return otp.ToString();

        }
      
    }
}
