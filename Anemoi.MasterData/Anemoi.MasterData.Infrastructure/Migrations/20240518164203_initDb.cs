using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anemoi.MasterData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Slug = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    SearchHint = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProvinceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Slug = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    SearchHint = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_Slug",
                table: "Districts",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Slug",
                table: "Provinces",
                column: "Slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
