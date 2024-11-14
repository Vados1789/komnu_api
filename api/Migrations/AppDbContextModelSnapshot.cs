﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using api.Data;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int?>("ParentCommentId")
                        .HasColumnType("int")
                        .HasColumnName("parent_comment_id");

                    b.Property<int>("PostId")
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("CommentId");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("comments");
                });

            modelBuilder.Entity("api.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("event_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int")
                        .HasColumnName("creator_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("event_date");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("event_name");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("location");

                    b.HasKey("EventId");

                    b.ToTable("events");
                });

            modelBuilder.Entity("api.Models.Friend", b =>
                {
                    b.Property<int>("FriendId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("friend_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FriendId"));

                    b.Property<DateTime>("RequestedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("requested_at")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("status");

                    b.Property<int>("UserId1")
                        .HasColumnType("int")
                        .HasColumnName("user_id_1");

                    b.Property<int>("UserId2")
                        .HasColumnType("int")
                        .HasColumnName("user_id_2");

                    b.HasKey("FriendId");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("friends");
                });

            modelBuilder.Entity("api.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("group_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int>("CreatorUserId")
                        .HasColumnType("int")
                        .HasColumnName("creator_user_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("group_name");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("image_url");

                    b.HasKey("GroupId");

                    b.HasIndex("CreatorUserId");

                    b.ToTable("groups");
                });

            modelBuilder.Entity("api.Models.GroupComment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("image_path");

                    b.Property<int?>("ParentCommentId")
                        .HasColumnType("int")
                        .HasColumnName("parent_comment_id");

                    b.Property<int>("PostId")
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("CommentId");

                    b.HasIndex("GroupId");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupComments");
                });

            modelBuilder.Entity("api.Models.GroupMember", b =>
                {
                    b.Property<int>("GroupMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("group_member_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupMemberId"));

                    b.Property<int>("GroupId")
                        .HasColumnType("int")
                        .HasColumnName("group_id");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("joined_at");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("GroupMemberId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("group_members");
                });

            modelBuilder.Entity("api.Models.GroupPost", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int>("GroupId")
                        .HasColumnType("int")
                        .HasColumnName("group_id");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("image_path");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("PostId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupPosts");
                });

            modelBuilder.Entity("api.Models.GroupReaction", b =>
                {
                    b.Property<int>("ReactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("reaction_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReactionId"));

                    b.Property<int?>("CommentId")
                        .HasColumnType("int")
                        .HasColumnName("comment_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int?>("PostId")
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    b.Property<string>("ReactionType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("reaction_type");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("ReactionId");

                    b.HasIndex("CommentId");

                    b.HasIndex("GroupId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupReactions");
                });

            modelBuilder.Entity("api.Models.Login", b =>
                {
                    b.Property<int>("LoginId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("login_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LoginId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsTwoFaEnabled")
                        .HasColumnType("bit")
                        .HasColumnName("is_two_fa_enabled");

                    b.Property<string>("LoginType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("login_type");

                    b.Property<string>("LoginValue")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("login_value");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("TwoFaMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("two_fa_method");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("LoginId");

                    b.HasIndex("UserId");

                    b.ToTable("logins");
                });

            modelBuilder.Entity("api.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("message_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit")
                        .HasColumnName("is_read");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("int")
                        .HasColumnName("receiver_id");

                    b.Property<int>("SenderId")
                        .HasColumnType("int")
                        .HasColumnName("sender_id");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("sent_at");

                    b.HasKey("MessageId");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("api.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("image_path");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("posts");
                });

            modelBuilder.Entity("api.Models.PostReaction", b =>
                {
                    b.Property<int>("ReactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("reaction_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReactionId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int>("PostId")
                        .HasColumnType("int")
                        .HasColumnName("post_id");

                    b.Property<string>("ReactionType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("reaction_type");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("ReactionId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("post_reactions");
                });

            modelBuilder.Entity("api.Models.TwoFaToken", b =>
                {
                    b.Property<int>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("token_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TokenId"));

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("expires_at");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)")
                        .HasColumnName("token");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("TokenId");

                    b.ToTable("two_fa_tokens");
                });

            modelBuilder.Entity("api.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("bio");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("date_of_birth");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("email");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("phone_number");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("profile_picture");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("username");

                    b.HasKey("UserId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("api.Models.UserRole", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("role_name");

                    b.HasKey("RoleId");

                    b.ToTable("user_roles");
                });

            modelBuilder.Entity("api.Models.UserRoleMapping", b =>
                {
                    b.Property<int>("UserRoleMappingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_role_mapping_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoleMappingId"));

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("UserRoleMappingId");

                    b.ToTable("user_roles_mapping");
                });

            modelBuilder.Entity("api.Models.Comment", b =>
                {
                    b.HasOne("api.Models.Comment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId");

                    b.HasOne("api.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ParentComment");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Friend", b =>
                {
                    b.HasOne("api.Models.User", "User1")
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User2")
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("api.Models.Group", b =>
                {
                    b.HasOne("api.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("api.Models.GroupComment", b =>
                {
                    b.HasOne("api.Models.Group", null)
                        .WithMany("GroupComments")
                        .HasForeignKey("GroupId");

                    b.HasOne("api.Models.GroupComment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("api.Models.GroupPost", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User")
                        .WithMany("GroupComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ParentComment");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.GroupMember", b =>
                {
                    b.HasOne("api.Models.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.GroupPost", b =>
                {
                    b.HasOne("api.Models.Group", "Group")
                        .WithMany("GroupPosts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User")
                        .WithMany("GroupPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.GroupReaction", b =>
                {
                    b.HasOne("api.Models.GroupComment", "Comment")
                        .WithMany("Reactions")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("api.Models.Group", null)
                        .WithMany("GroupReactions")
                        .HasForeignKey("GroupId");

                    b.HasOne("api.Models.GroupPost", "Post")
                        .WithMany("Reactions")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("api.Models.User", "User")
                        .WithMany("GroupReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Login", b =>
                {
                    b.HasOne("api.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Post", b =>
                {
                    b.HasOne("api.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.PostReaction", b =>
                {
                    b.HasOne("api.Models.Post", "Post")
                        .WithMany("PostReactions")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.User", "User")
                        .WithMany("PostReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Comment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("api.Models.Group", b =>
                {
                    b.Navigation("GroupComments");

                    b.Navigation("GroupPosts");

                    b.Navigation("GroupReactions");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("api.Models.GroupComment", b =>
                {
                    b.Navigation("Reactions");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("api.Models.GroupPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("api.Models.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("PostReactions");
                });

            modelBuilder.Entity("api.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("GroupComments");

                    b.Navigation("GroupMemberships");

                    b.Navigation("GroupPosts");

                    b.Navigation("GroupReactions");

                    b.Navigation("PostReactions");
                });
#pragma warning restore 612, 618
        }
    }
}
