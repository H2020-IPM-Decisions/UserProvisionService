using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ExtraColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultMessage",
                table: "FieldDssResult",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultMessageType",
                table: "FieldDssResult",
                nullable: true);            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultMessage",
                table: "FieldDssResult");

            migrationBuilder.DropColumn(
                name: "ResultMessageType",
                table: "FieldDssResult");
        }
    }
}
