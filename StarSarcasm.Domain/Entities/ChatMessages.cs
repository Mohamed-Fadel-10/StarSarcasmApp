using StarSarcasm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class ChatMessages
    {
        public int Id { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsReaded { get; set; }
        public DateTime SendAt { get; set; }
        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        [ForeignKey("Reciver")]
        public string ReciverId { get; set; }
        [ForeignKey("Chat")]
        public string ChatId { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Reciver { get; set; }

        public virtual Chat Chat { get; set; }

    }
}
