    using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Application.Response;
using StarSarcasm.Application.TwilioSettings;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace StarSarcasm.Infrastructure.Services.SMSServices
{
    public class OTPService : IOTPService
    {
        private readonly TwilioSettings _twilio;

        public OTPService(IOptions<TwilioSettings> twilio, Context _context, UserManager<ApplicationUser> _userManager)
        {
            _twilio = twilio.Value;
        }
        public string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public MessageResource Send(string mobileNumber, string body)
        {
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
            var result=MessageResource.Create(
                body:body,
                from:new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                to:mobileNumber
                );
            return result;
        }

      
    }
}
