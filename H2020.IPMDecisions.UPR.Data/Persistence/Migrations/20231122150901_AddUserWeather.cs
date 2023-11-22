using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddUserWeather : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserWeatherId",
                table: "Farm",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserWeather",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    WeatherId = table.Column<string>(nullable: false),
                    WeatherStationId = table.Column<string>(nullable: false),
                    WeatherStationReference = table.Column<string>(nullable: false),
                    UserProfileId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWeather", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWeather_UserProfile",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farm_UserWeatherId",
                table: "Farm",
                column: "UserWeatherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeather_UserProfileId",
                table: "UserWeather",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_UserWeather_UserWeatherId",
                table: "Farm",
                column: "UserWeatherId",
                principalTable: "UserWeather",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_UserWeather_UserWeatherId",
                table: "Farm");

            migrationBuilder.DropTable(
                name: "UserWeather");

            migrationBuilder.DropIndex(
                name: "IX_Farm_UserWeatherId",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "UserWeatherId",
                table: "Farm");
        }
    }
}
