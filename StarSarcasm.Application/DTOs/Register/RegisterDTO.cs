using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs.Register
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Please,enter a valid email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password,ErrorMessage =
            "Password must contain uppercase and lowercase letters, numbers and special characters.")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Not Matched!")]
        public string ConfirmPassword { get; set; }

        public string FcmToken { get; set; }

    }
}
