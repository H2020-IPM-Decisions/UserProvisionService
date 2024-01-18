using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class UniqueDSSrestrain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssVersion", "DssModelId", "DssModelVersion", "DssExecutionType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId", "DssModelVersion", "DssExecutionType" },
                unique: true);
        }
    }
}
