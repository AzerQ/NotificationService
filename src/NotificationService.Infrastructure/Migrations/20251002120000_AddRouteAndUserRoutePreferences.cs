using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Migrations
{
    public partial class AddRouteAndUserRoutePreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "Notifications",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserRoutePreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Route = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoutePreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoutePreferences_UserId_Route",
                table: "UserRoutePreferences",
                columns: new[] { "UserId", "Route" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoutePreferences");

            migrationBuilder.DropColumn(
                name: "Route",
                table: "Notifications");
        }
    }
}
