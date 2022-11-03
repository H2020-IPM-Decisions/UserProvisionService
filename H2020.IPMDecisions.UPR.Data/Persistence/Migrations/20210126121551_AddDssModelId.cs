using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddDssModelId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_CropPestId_DssName",
                table: "CropPestDss");

            migrationBuilder.AddColumn<string>(
                name: "DssModelId",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DssModelName",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssModelId",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssModelName",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssName",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssName" },
                unique: true);
        }
    }
}
