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
        public string Phone { get; set; }
    }
}
