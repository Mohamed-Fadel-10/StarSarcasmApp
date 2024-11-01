using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsSubscribed { get; set; }
        public string FcmToken { get; set; }
        public string BirthDate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool? IsLoggedIn { get; set; }
    }
}
