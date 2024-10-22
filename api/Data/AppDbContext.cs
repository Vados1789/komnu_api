using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define DbSets for all models
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRoleMapping> UserRoleMappings { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<TwoFaToken> TwoFaTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between Post and User
            //modelBuilder.Entity<Post>()
            //    .HasOne(p => p.User) // Each Post has one User
            //    .WithMany() // Assuming a User can have many Posts
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.Cascade); // Optional: Configure cascade delete

            //// Configure relationships for Comment and Post/User
            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.User) // Each Comment has one User
            //    .WithMany() // Assuming a User can have many Comments
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Cascade); // Optional: Configure cascade delete

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.Post) // Each Comment has one Post
            //    .WithMany() // Assuming a Post can have many Comments
            //    .HasForeignKey(c => c.PostId)
            //    .OnDelete(DeleteBehavior.Cascade); // Optional: Configure cascade delete

            //// Example of User Role Mapping Configuration
            //modelBuilder.Entity<UserRoleMapping>()
            //    .HasOne(urm => urm.User)
            //    .WithMany() // Assuming a User can have many roles
            //    .HasForeignKey(urm => urm.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<UserRoleMapping>()
            //    .HasOne(urm => urm.Role)
            //    .WithMany() // Assuming a Role can be assigned to many users
            //    .HasForeignKey(urm => urm.RoleId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Configure relationships for Friends
            //modelBuilder.Entity<Friend>()
            //    .HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(f => f.UserId1)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            //modelBuilder.Entity<Friend>()
            //    .HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(f => f.UserId2)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            //// Configure relationships for GroupMember and Group/User
            //modelBuilder.Entity<GroupMember>()
            //    .HasOne(gm => gm.Group)
            //    .WithMany() // Assuming a Group can have many members
            //    .HasForeignKey(gm => gm.GroupId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<GroupMember>()
            //    .HasOne(gm => gm.User)
            //    .WithMany() // Assuming a User can join many groups
            //    .HasForeignKey(gm => gm.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Configure Message relationships
            //modelBuilder.Entity<Message>()
            //    .HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(m => m.SenderId)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            //modelBuilder.Entity<Message>()
            //    .HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(m => m.ReceiverId)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            //// Configure Event relationship with User
            //modelBuilder.Entity<Event>()
            //    .HasOne(e => e.User)
            //    .WithMany() // Assuming a User can create many Events
            //    .HasForeignKey(e => e.CreatorId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
