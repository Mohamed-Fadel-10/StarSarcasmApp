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
        [ForeignKey("Sender")]

        public string User1 { get; set; }
        [ForeignKey("Receiver")]

        public string User2 { get; set; }
        [ForeignKey("Chat")]

        public int ChatId { get; set; }
        public ApplicationUser Sender { get; set; } // Sender
        public ApplicationUser Receiver { get; set; } // Receiver
        public Chat Chat { get; set; }
    }
}