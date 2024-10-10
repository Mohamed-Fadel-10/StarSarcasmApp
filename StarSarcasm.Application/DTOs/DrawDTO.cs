using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class DrawDTO
    {
        [Required(ErrorMessage = "Name Filed is Required")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "StartAt Filed is Required")]
        public DateTime StartAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "EndAt Filed is Required")]
        public DateTime EndAt { get; set; }
        public IFormFile? file { get; set; }

    }
}
