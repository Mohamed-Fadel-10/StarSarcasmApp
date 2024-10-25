using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class UserChatDTO
    {
        public string ChatId { get; set; }
        public string ChatName { get; set; }
        public DateTime ReceiverDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiverId { get; set; }
        public string FcmToken { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; }
    }
}
