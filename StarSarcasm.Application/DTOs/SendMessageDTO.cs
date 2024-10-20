using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class SendMessageDTO
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public DateTime SendAt { get; set;}
        public string SenderId { get; set;}
        public string ReciverId { get; set;}
        public int ChatId { get; set;}

        public IFormFile? VoiceFile { get; set;}
    }
}
