using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class NewDssInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssModelName",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssName",
                table: "CropPestDss");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "FieldDssResult",
                newName: "DssFullResult");

            migrationBuilder.AddColumn<string>(
                name: "WarningMessage",
                table: "FieldDssResult",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarningStatus",
                table: "FieldDssResult",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DssEndPoint",
                table: "CropPestDss",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarningMessage",
                table: "FieldDssResult");

            migrationBuilder.DropColumn(
                name: "WarningStatus",
                table: "FieldDssResult");

            migrationBuilder.DropColumn(
                name: "DssEndPoint",
                table: "CropPestDss");

            migrationBuilder.RenameColumn(
                name: "DssFullResult",
                table: "FieldDssResult",
                newName: "Result");

            migrationBuilder.AddColumn<string>(
                name: "DssModelName",
                table: "CropPestDss",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DssName",
                table: "CropPestDss",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
