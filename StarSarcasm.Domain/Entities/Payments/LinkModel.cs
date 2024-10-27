using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Domain.Entities.Payments
{
	public class LinkModel
	{
        public string? method { get; set; }
        public string? rel { get; set; }
        public string? href { get; set; }
    }
}
