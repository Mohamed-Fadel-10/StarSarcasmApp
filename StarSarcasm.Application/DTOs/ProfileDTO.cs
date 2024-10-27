using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class ProfileDTO
    {
        public string? Name { get; set; }
        public string? BirthDate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? Location { get; set; }
    }
}
