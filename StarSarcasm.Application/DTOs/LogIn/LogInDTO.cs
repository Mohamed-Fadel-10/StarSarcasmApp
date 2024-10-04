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

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public string DeviceIPAddress { get; set; }

    }
}
