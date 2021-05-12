using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddDssNameAndModelNameToTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DssModelName",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DssName",
                table: "CropPestDss",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssModelName",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssName",
                table: "CropPestDss");
        }
    }
}
