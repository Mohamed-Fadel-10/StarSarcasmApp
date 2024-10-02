using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarSarcasm.Domain.Entities;
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

            base.OnModelCreating(builder);

        }

        public DbSet<Draw> Draws { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UsersDraws> UsersDraws { get; set; }
        public DbSet<UsersMessages> UsersMessages { get; set; }

    }
}
