using Microsoft.AspNetCore.Identity;
using StarSarcasm.Domain.Entities.Payments;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSarcasm.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string FcmToken { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public virtual ICollection<UsersDraws>? UsersDraws { get; set; } = new List<UsersDraws>();
        public virtual ICollection<UsersMessages>? UsersMessages { get; set; } = new List<UsersMessages>();
        public virtual ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Subscription>? Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<ChatMessages>? SentMessages { get; set; }= new List<ChatMessages>();
        public virtual ICollection<ChatMessages>? ReceivedMessages { get; set; }= new List<ChatMessages>();
        public virtual ICollection<UsersChats> SentChats { get; set; } = new List<UsersChats>(); // Chats where user is the sender
        public virtual ICollection<UsersChats> ReceivedChats { get; set; } = new List<UsersChats>(); // Chats where user is the receiver

    }
}
