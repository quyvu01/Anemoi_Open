using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anemoi.Workspace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogoPath = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SearchHint = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CreatorId = table.Column<string>(type: "text", nullable: true),
                    RoleGroupIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberInvitations_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    IsActivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    LogoPath = table.Column<string>(type: "text", nullable: true),
                    CoverPhotoPath = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SearchHint = table.Column<string>(type: "text", nullable: true),
                    TaxCode = table.Column<string>(type: "text", nullable: true),
                    ProvinceId = table.Column<string>(type: "text", nullable: true),
                    DistrictId = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    SubDomain = table.Column<string>(type: "text", nullable: true),
                    CustomDomain = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Organizations_ParentOrganizationId",
                        column: x => x.ParentOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Organizations_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberMapRoleGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleGroupId = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMapRoleGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberMapRoleGroups_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberInvitationMapOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberInvitationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInvitationMapOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberInvitationMapOrganizations_MemberInvitations_MemberIn~",
                        column: x => x.MemberInvitationId,
                        principalTable: "MemberInvitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberInvitationMapOrganizations_Organizations_Organization~",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberMapOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMapOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberMapOrganizations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberMapOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitationMapOrganizations_MemberInvitationId",
                table: "MemberInvitationMapOrganizations",
                column: "MemberInvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitationMapOrganizations_OrganizationId",
                table: "MemberInvitationMapOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_Email_WorkspaceId",
                table: "MemberInvitations",
                columns: new[] { "Email", "WorkspaceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_WorkspaceId",
                table: "MemberInvitations",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMapOrganizations_MemberId",
                table: "MemberMapOrganizations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMapOrganizations_OrganizationId",
                table: "MemberMapOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMapRoleGroups_MemberId",
                table: "MemberMapRoleGroups",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_WorkspaceId",
                table: "Members",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentOrganizationId",
                table: "Organizations",
                column: "ParentOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_SubDomain",
                table: "Organizations",
                column: "SubDomain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_WorkspaceId",
                table: "Organizations",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberInvitationMapOrganizations");

            migrationBuilder.DropTable(
                name: "MemberMapOrganizations");

            migrationBuilder.DropTable(
                name: "MemberMapRoleGroups");

            migrationBuilder.DropTable(
                name: "MemberInvitations");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Workspaces");
        }
    }
}
