using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveGenericFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "ObservationResult");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "ObservationResult");

            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "ForecastResult");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "ForecastResult");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "CropPestDssResult");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "CropPestDssResult");

            migrationBuilder.AddColumn<string>(
                name: "Variety",
                table: "Field",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variety",
                table: "Field");

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "ObservationResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "ObservationResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "ForecastResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "ForecastResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "Field",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "CropPestDssResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "CropPestDssResult",
                type: "text",
                nullable: true);
        }
    }
}
