using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class UsersChats
    {
        public int Id { get; set; }
        [ForeignKey("User")]

        public string UserId { get; set; }
        [ForeignKey("Chat")]

        public int ChatId { get; set; }
        public ApplicationUser User { get; set; }
        public Chat Chat { get; set; }
    }
}
