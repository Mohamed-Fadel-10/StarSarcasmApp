using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class ChatDTO
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public DateTime ReceiverDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiverId { get; set; }
        public string FcmToken { get; set; }
    }
}
