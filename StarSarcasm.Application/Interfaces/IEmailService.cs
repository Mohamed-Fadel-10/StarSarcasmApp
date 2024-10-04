using StarSarcasm.Application.DTOs.LogIn;
using StarSarcasm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(LogInDTO user, string otp);

    }
}
