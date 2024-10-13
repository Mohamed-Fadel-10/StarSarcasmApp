using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class UsersDraws
    {
        [Key]
        public int Id { get; set; }
        public DateTime? LastWinDate { get; set; }

        public bool IsWinner { get; set; }

        public string UserId { get; set; }
        public int DrawId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("DrawId")]
        public virtual Draw Draw { get; set; }
    }
}
