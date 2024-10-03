using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs.LogIn
{
    public class LogInDTO
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [PhoneNumberValidation(ErrorMessage = "Please enter a valid phone number with the correct country code.")]
        public string PhoneNumber { get; set; }

        public string DeviceIPAddress { get; set; }

    }
}
