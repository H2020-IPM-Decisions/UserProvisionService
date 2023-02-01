using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddColumnsToAllowSaveCustomDss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId",
                table: "FieldCropPestDss");

            migrationBuilder.AddColumn<string>(
                name: "CustomName",
                table: "FieldCropPestDss",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomDss",
                table: "FieldCropPestDss",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId_IsCustomDss_~",
                table: "FieldCropPestDss",
                columns: new[] { "FieldCropPestId", "CropPestDssId", "IsCustomDss", "CustomName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId_IsCustomDss_~",
                table: "FieldCropPestDss");

            migrationBuilder.DropColumn(
                name: "CustomName",
                table: "FieldCropPestDss");

            migrationBuilder.DropColumn(
                name: "IsCustomDss",
                table: "FieldCropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId",
                table: "FieldCropPestDss",
                columns: new[] { "FieldCropPestId", "CropPestDssId" },
                unique: true);
        }
    }
}
