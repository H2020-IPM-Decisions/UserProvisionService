using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveUniqueUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_Farm",
                table: "UserFarm");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_User",
                table: "UserFarm");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserFarm",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "UserFarm",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserFarm_UserProfileId",
                table: "UserFarm",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_Farm_FarmId",
                table: "UserFarm",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_UserProfile_UserProfileId",
                table: "UserFarm",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_Farm_FarmId",
                table: "UserFarm");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_UserProfile_UserProfileId",
                table: "UserFarm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm");

            migrationBuilder.DropIndex(
                name: "IX_UserFarm_UserProfileId",
                table: "UserFarm");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserFarm");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "UserFarm");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm",
                columns: new[] { "UserId", "FarmId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId",
                unique: true);

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
    }
}
