using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ExtraInformationToDss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DssExecutionType",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DssVersion",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssExecutionType",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssVersion",
                table: "CropPestDss");
        }
    }
}
