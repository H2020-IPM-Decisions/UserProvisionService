using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveColumnVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId_DssVersion_DssExecu~",
                table: "CropPestDss");

            migrationBuilder.DropColumn(
                name: "DssVersion",
                table: "CropPestDss");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId", "DssModelVersion", "DssExecutionType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CropPestDss_All",
                table: "CropPestDss");

            migrationBuilder.AddColumn<string>(
                name: "DssVersion",
                table: "CropPestDss",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssId_DssModelId_DssVersion_DssExecu~",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssId", "DssModelId", "DssVersion", "DssExecutionType" },
                unique: true);
        }
    }
}
