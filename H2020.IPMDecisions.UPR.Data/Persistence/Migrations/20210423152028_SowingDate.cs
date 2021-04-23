using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class SowingDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "Field");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "FieldDssResult",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SowingDate",
                table: "Field",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "FieldDssResult");

            migrationBuilder.DropColumn(
                name: "SowingDate",
                table: "Field");

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "Field",
                type: "text",
                nullable: true);
        }
    }
}
