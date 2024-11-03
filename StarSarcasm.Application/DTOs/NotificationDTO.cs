using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class NotificationDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public string UserId { get; set; }
        public DateTime SentAt { get; set; }

    }
}
