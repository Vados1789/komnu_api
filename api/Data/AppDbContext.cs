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
        public DbSet<GroupPost> GroupPosts { get; set; }
        public DbSet<GroupComment> GroupComments { get; set; }
        public DbSet<GroupReaction> GroupReactions { get; set; }
        public DbSet<TwoFaToken> TwoFaTokens { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friend>()
               .HasOne(f => f.User2)
               .WithMany()
               .HasForeignKey(f => f.UserId2)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .Property(f => f.RequestedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)  // Assuming Post has a Comments collection
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)  // Assuming User has a Comments collection
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure default value for CreatedAt in Comment entity
            modelBuilder.Entity<Comment>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.Post)
                .WithMany(p => p.PostReactions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupPost>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPosts)
                .HasForeignKey(gp => gp.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupPost>()
                .HasOne(gp => gp.User)
                .WithMany(u => u.GroupPosts)
                .HasForeignKey(gp => gp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupComment>()
                .HasOne(gc => gc.Post)
                .WithMany(gp => gp.Comments)
                .HasForeignKey(gc => gc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupComment>()
                .HasOne(gc => gc.User)
                .WithMany(u => u.GroupComments)
                .HasForeignKey(gc => gc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupComment>()
                .HasOne(gc => gc.ParentComment)
                .WithMany(pc => pc.Replies)
                .HasForeignKey(gc => gc.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupReaction>()
                .HasOne(gr => gr.Post)
                .WithMany(gp => gp.Reactions)
                .HasForeignKey(gr => gr.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupReaction>()
                .HasOne(gr => gr.Comment)
                .WithMany(gc => gc.Reactions)
                .HasForeignKey(gr => gr.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupReaction>()
                .HasOne(gr => gr.User)
                .WithMany(u => u.GroupReactions)
                .HasForeignKey(gr => gr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey(g => g.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");

            modelBuilder.Entity<GroupMember>()
                .Property(gm => gm.JoinedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");
        }
    }
}
