using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anemoi.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    PasswordBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    Host = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    IsSslEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SearchHint = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<string>(type: "text", nullable: true),
                    TargetUserId = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsTrash = table.Column<bool>(type: "boolean", nullable: false),
                    Parameters = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfigurations_WorkspaceId",
                table: "EmailConfigurations",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserId",
                table: "Notifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserId_WorkspaceId",
                table: "Notifications",
                columns: new[] { "TargetUserId", "WorkspaceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailConfigurations");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");
        }
    }
}
