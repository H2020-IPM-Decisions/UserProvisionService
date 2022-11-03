using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddFKAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_Farm_FarmId",
                table: "UserFarm");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_UserProfile_UserId",
                table: "UserFarm");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_Farm",
                table: "UserFarm",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_User",
                table: "UserFarm",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_Farm",
                table: "UserFarm");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_User",
                table: "UserFarm");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_Farm_FarmId",
                table: "UserFarm",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_UserProfile_UserId",
                table: "UserFarm",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
