using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class Chat
    {
        [Key]
        public string Id { get; set; }
        public string ReciverChatName { get; set; }
        public string SenderChatName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ChatMessages>? ChatMessages { get; set; } = new List<ChatMessages>();
        public virtual ICollection<UsersChats>? UsersChats { get; set; } = new List<UsersChats>();

    }
}
