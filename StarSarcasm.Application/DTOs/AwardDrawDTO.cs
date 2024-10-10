using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class AwardDrawDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool IsActive => DateTime.Now >= StartAt
            && DateTime.Now <= EndAt ? true : false;
        public string? ImagePath { get; set; }

    }
}
