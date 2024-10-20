using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Data
{
    public class Context:IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions<Context> options):base(options)
        {
            
        }
        public Context()
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UsersDraws>()
               .HasKey(ud => ud.Id);

            builder.Entity<UsersDraws>()
                .HasOne(ud => ud.Draw)
                .WithMany(ud => ud.UsersDraws)
                .HasForeignKey(ud => ud.DrawId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            builder.Entity<UsersDraws>()
                .HasOne(ud => ud.User)
                .WithMany(ud => ud.UsersDraws)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            builder.Entity<UsersMessages>()
                .HasKey(um =>um.Id);
             
            builder.Entity<UsersMessages>()
                .HasOne(um=>um.Message)
                .WithMany(um=>um.UsersMessages)
                .HasForeignKey(um=>um.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UsersMessages>()
                .HasOne(um => um.User)
                .WithMany(um => um.UsersMessages)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Chat>()
               .Property(c => c.Id)
               .ValueGeneratedNever();


            builder.Entity<ChatMessages>()
           .HasOne(c => c.Sender)
           .WithMany(u => u.SentMessages)
           .HasForeignKey(c => c.SenderId)
           .OnDelete(DeleteBehavior.NoAction);

            // Configuring the relationship between ApplicationUser and ChatMessages (Received Messages)
            builder.Entity<ChatMessages>()
                .HasOne(c => c.Reciver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(c => c.ReciverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessages>()
                .HasOne(c => c.Chat)
                .WithMany(c => c.ChatMessages)
                .HasForeignKey(c => c.ChatId);

            builder.Entity<UsersChats>()
        .HasOne(uc => uc.Sender)
        .WithMany(au => au.SentChats)
        .HasForeignKey(uc => uc.User1)
        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersChats>()
                .HasOne(uc => uc.Receiver)
                .WithMany(au => au.ReceivedChats)
                .HasForeignKey(uc => uc.User2)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersChats>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.UsersChats)
                .HasForeignKey(uc => uc.ChatId);


            base.OnModelCreating(builder);

        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Draw> Draws { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UsersDraws> UsersDraws { get; set; }
        public DbSet<UsersMessages> UsersMessages { get; set; }
        public DbSet<OTP> OTP { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<UsersChats> UsersChats { get; set; }
        public DbSet<ChatMessages> ChatMessages { get; set; }
    }
}
