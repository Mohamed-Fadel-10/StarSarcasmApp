using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities
{
    public class Zodiac
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Index { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; } = new List<ApplicationUser>();
    }
}
