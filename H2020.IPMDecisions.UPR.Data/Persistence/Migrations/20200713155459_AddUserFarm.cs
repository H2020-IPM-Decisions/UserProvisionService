using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddUserFarm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "Farm",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Inf1 = table.Column<string>(nullable: true),
                    Inf2 = table.Column<string>(nullable: true),
                    Location = table.Column<Point>(type: "geometry (point)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFarm",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    FarmId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFarm", x => new { x.UserId, x.FarmId });
                    table.ForeignKey(
                        name: "FK_UserFarm_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFarm_User",
                        column: x => x.UserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farm_Location",
                table: "Farm",
                column: "Location",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFarm_FarmId",
                table: "UserFarm",
                column: "FarmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFarm");

            migrationBuilder.DropTable(
                name: "Farm");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile");
        }
    }
}
