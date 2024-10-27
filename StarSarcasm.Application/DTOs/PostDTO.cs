using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class PostDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? VideoPath { get; set; }
        public IFormFile? ImagePath { get; set; }
    }
}
