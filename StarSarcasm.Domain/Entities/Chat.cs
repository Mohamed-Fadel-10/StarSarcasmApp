using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ChatMessages>? ChatMessages { get; set; } = new List<ChatMessages>();
        public virtual ICollection<UsersChats>? UsersChats { get; set; } = new List<UsersChats>();

    }
}
