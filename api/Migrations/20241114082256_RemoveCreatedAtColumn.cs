using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    creator_id = table.Column<int>(type: "int", nullable: false),
                    event_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    event_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    receiver_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.message_id);
                });

            migrationBuilder.CreateTable(
                name: "two_fa_tokens",
                columns: table => new
                {
                    token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_two_fa_tokens", x => x.token_id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles_mapping",
                columns: table => new
                {
                    user_role_mapping_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles_mapping", x => x.user_role_mapping_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    profile_picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    friend_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id_1 = table.Column<int>(type: "int", nullable: false),
                    user_id_2 = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friends", x => x.friend_id);
                    table.ForeignKey(
                        name: "FK_friends_users_user_id_1",
                        column: x => x.user_id_1,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friends_users_user_id_2",
                        column: x => x.user_id_2,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    creator_user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.group_id);
                    table.ForeignKey(
                        name: "FK_groups_users_creator_user_id",
                        column: x => x.creator_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "logins",
                columns: table => new
                {
                    login_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    login_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    login_value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_two_fa_enabled = table.Column<bool>(type: "bit", nullable: false),
                    two_fa_method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logins", x => x.login_id);
                    table.ForeignKey(
                        name: "FK_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    group_member_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_members", x => x.group_member_id);
                    table.ForeignKey(
                        name: "FK_group_members_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPosts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPosts", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_GroupPosts_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPosts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    parent_comment_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_comments_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_comments_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_reactions",
                columns: table => new
                {
                    reaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    reaction_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_reactions", x => x.reaction_id);
                    table.ForeignKey(
                        name: "FK_post_reactions_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_reactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupComments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    parent_comment_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupComments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_GroupComments_GroupComments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "GroupComments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_GroupComments_GroupPosts_post_id",
                        column: x => x.post_id,
                        principalTable: "GroupPosts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupComments_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_GroupComments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "GroupReactions",
                columns: table => new
                {
                    reaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    comment_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    reaction_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupReactions", x => x.reaction_id);
                    table.ForeignKey(
                        name: "FK_GroupReactions_GroupComments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "GroupComments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_GroupReactions_GroupPosts_post_id",
                        column: x => x.post_id,
                        principalTable: "GroupPosts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK_GroupReactions_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_GroupReactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_post_id",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_friends_user_id_1",
                table: "friends",
                column: "user_id_1");

            migrationBuilder.CreateIndex(
                name: "IX_friends_user_id_2",
                table: "friends",
                column: "user_id_2");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_group_id",
                table: "group_members",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_user_id",
                table: "group_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupComments_GroupId",
                table: "GroupComments",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupComments_parent_comment_id",
                table: "GroupComments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupComments_post_id",
                table: "GroupComments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupComments_user_id",
                table: "GroupComments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPosts_group_id",
                table: "GroupPosts",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPosts_user_id",
                table: "GroupPosts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupReactions_comment_id",
                table: "GroupReactions",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupReactions_GroupId",
                table: "GroupReactions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupReactions_post_id",
                table: "GroupReactions",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupReactions_user_id",
                table: "GroupReactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_creator_user_id",
                table: "groups",
                column: "creator_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_logins_user_id",
                table: "logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_reactions_post_id",
                table: "post_reactions",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_reactions_user_id",
                table: "post_reactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_user_id",
                table: "posts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "group_members");

            migrationBuilder.DropTable(
                name: "GroupReactions");

            migrationBuilder.DropTable(
                name: "logins");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "post_reactions");

            migrationBuilder.DropTable(
                name: "two_fa_tokens");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_roles_mapping");

            migrationBuilder.DropTable(
                name: "GroupComments");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "GroupPosts");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
