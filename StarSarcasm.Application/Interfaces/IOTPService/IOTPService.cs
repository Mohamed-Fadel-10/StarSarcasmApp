using StarSarcasm.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace StarSarcasm.Application.Interfaces.ISMSService
{
    public interface IOTPService
    {
        public MessageResource Send(string mobileNumber, string body);
        public string GenerateOTP();
       
    }
}
