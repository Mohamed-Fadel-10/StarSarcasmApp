using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class ChangePasswordDTO
    {
        
        public string Email { get; set; }

        [Required(ErrorMessage ="Is required!")]
        [PasswordPropertyText]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword",ErrorMessage ="Not matched!")]
        public string ConfirmNewPassword { get; set; }
    }
}
