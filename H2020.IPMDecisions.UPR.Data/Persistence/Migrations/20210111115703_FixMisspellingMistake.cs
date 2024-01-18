using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class FixMisspellingMistake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spray_FieldCropPest",
                table: "FieldSprayApplication");

            migrationBuilder.DropIndex(
                name: "IX_FieldSprayApplication_FieldCropPestdId",
                table: "FieldSprayApplication");

            migrationBuilder.DropColumn(
                name: "FieldCropPestdId",
                table: "FieldSprayApplication");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropPestId",
                table: "FieldSprayApplication",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldSprayApplication_FieldCropPestId",
                table: "FieldSprayApplication",
                column: "FieldCropPestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spray_FieldCropPest",
                table: "FieldSprayApplication",
                column: "FieldCropPestId",
                principalTable: "FieldCropPest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spray_FieldCropPest",
                table: "FieldSprayApplication");

            migrationBuilder.DropIndex(
                name: "IX_FieldSprayApplication_FieldCropPestId",
                table: "FieldSprayApplication");

            migrationBuilder.DropColumn(
                name: "FieldCropPestId",
                table: "FieldSprayApplication");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropPestdId",
                table: "FieldSprayApplication",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldSprayApplication_FieldCropPestdId",
                table: "FieldSprayApplication",
                column: "FieldCropPestdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spray_FieldCropPest",
                table: "FieldSprayApplication",
                column: "FieldCropPestdId",
                principalTable: "FieldCropPest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
