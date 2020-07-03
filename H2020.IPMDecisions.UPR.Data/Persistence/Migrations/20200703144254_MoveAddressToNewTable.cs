using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class MoveAddressToNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "UserProfile");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAddressId",
                table: "UserProfile",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Street = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Postcode = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserAddressId",
                table: "UserProfile",
                column: "UserAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_UserAddress_UserAddressId",
                table: "UserProfile",
                column: "UserAddressId",
                principalTable: "UserAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_UserAddress_UserAddressId",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_UserAddressId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "UserAddressId",
                table: "UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "UserProfile",
                type: "text",
                nullable: true);
        }
    }
}
