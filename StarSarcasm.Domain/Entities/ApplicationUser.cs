using Microsoft.AspNetCore.Identity;
using System;

namespace StarSarcasm.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string FcmToken { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public string Location { get; set; }

        public virtual ICollection<UsersDraws>? UsersDraws { get; set; } = new List<UsersDraws>();
        public virtual ICollection<UsersMessages>? UsersMessages { get; set; } = new List<UsersMessages>();
        public virtual ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Subscription>? Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
