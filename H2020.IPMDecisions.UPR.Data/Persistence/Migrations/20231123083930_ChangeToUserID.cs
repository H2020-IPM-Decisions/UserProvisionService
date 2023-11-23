using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ChangeToUserID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWeather_UserProfile",
                table: "UserWeather");

            migrationBuilder.DropIndex(
                name: "IX_UserWeather_UserProfileId",
                table: "UserWeather");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "UserWeather");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserWeather",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserWeather_UserId",
                table: "UserWeather",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWeather_UserProfile",
                table: "UserWeather",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWeather_UserProfile",
                table: "UserWeather");

            migrationBuilder.DropIndex(
                name: "IX_UserWeather_UserId",
                table: "UserWeather");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserWeather");

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "UserWeather",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserWeather_UserProfileId",
                table: "UserWeather",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWeather_UserProfile",
                table: "UserWeather",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
