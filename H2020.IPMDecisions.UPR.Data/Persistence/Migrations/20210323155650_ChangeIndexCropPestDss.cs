using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ChangeIndexCropPestDss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId_DssVersion_DssExecu~",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId", "DssVersion", "DssExecutionType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId_DssVersion_DssExecu~",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId" },
                unique: true);
        }
    }
}
