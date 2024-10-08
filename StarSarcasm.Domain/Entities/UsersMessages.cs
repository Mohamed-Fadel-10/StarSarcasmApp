using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class UsersMessages
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MessageId { get; set; }
        public DateTime SendAt { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }
    }
}
