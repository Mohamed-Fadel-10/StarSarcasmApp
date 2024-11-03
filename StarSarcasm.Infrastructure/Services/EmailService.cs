using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Application.DTOs.Register;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.Email;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly Context _context;
        private readonly IOTPService _oTPService;

		public EmailService(IOptions<EmailSettings> options, Context context, IOTPService oTPService = null)
		{
			_emailSettings = options.Value;
			_context = context;
			_oTPService = oTPService;
		}

		private string OtpBodyGenerator(string otp)
        {
            string body = "<div>";
            body += "<h3>Hello,</h3>";
            body += "<h5>Please use the following OTP to verify your email address: </h5>";
            body += "<h1>" + otp + "</h1>";
            body += "<br><h5>Have a nice day,</h5>";
            body += "<h6>Hazzy Support Team.</h6>";
            body += "</div>";
            return body;
        }

        public async Task SendOtpAsync(string userEmail,string otp)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_emailSettings.Email),
                Subject = "OTP Email Verfication"
            };

            email.To.Add(MailboxAddress.Parse(userEmail));
            var builder = new BodyBuilder();

            builder.HtmlBody=OtpBodyGenerator(otp);
            email.Body=builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

		public async Task<ResponseModel> ReSendOtpAsync(string userEmail)
        {
            if (userEmail != null)
            {
                var otps = await _context.OTP.FirstOrDefaultAsync(o => o.ExpirationTime > DateTime.UtcNow && o.Email == userEmail);

                if (otps != null)
                {
                    _context.OTP.RemoveRange(otps);
                }

                var otp = await _oTPService.GenerateOTP(userEmail);
                await SendOtpAsync(userEmail, otp);

                return new ResponseModel
                {
                    Message = "OTP resent successfully, it is valid for 5 min.",
                    StatusCode = 200,
                    IsSuccess = true,
                };
            }
            return new ResponseModel
            {
                Message = "invalid Email Address",
                StatusCode = 400,
                IsSuccess = false,
            };
        }
         
    }
}
