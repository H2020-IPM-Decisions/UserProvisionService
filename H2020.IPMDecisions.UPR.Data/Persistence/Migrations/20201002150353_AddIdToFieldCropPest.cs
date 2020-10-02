using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddIdToFieldCropPest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldCropPest",
                table: "FieldCropPest");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FieldCropPest",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldCropPest",
                table: "FieldCropPest",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPest_FieldId_CropPestId",
                table: "FieldCropPest",
                columns: new[] { "FieldId", "CropPestId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldCropPest",
                table: "FieldCropPest");

            migrationBuilder.DropIndex(
                name: "IX_FieldCropPest_FieldId_CropPestId",
                table: "FieldCropPest");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FieldCropPest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldCropPest",
                table: "FieldCropPest",
                columns: new[] { "FieldId", "CropPestId" });
        }
    }
}
