using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class NewFieldCrop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CropPest_Field",
                table: "FieldCropPest");

            migrationBuilder.DropIndex(
                name: "IX_FieldCropPest_FieldId_CropPestId",
                table: "FieldCropPest");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "FieldCropPest");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropId",
                table: "FieldCropPest",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "FieldCrop",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CropEppoCode = table.Column<string>(maxLength: 6, nullable: false),
                    FieldId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCrop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Field_FieldCrop",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPest_FieldCropId_CropPestId",
                table: "FieldCropPest",
                columns: new[] { "FieldCropId", "CropPestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldCrop_FieldId",
                table: "FieldCrop",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldCropPest_FieldCrop_FieldCropId",
                table: "FieldCropPest",
                column: "FieldCropId",
                principalTable: "FieldCrop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldCropPest_FieldCrop_FieldCropId",
                table: "FieldCropPest");

            migrationBuilder.DropTable(
                name: "FieldCrop");

            migrationBuilder.DropIndex(
                name: "IX_FieldCropPest_FieldCropId_CropPestId",
                table: "FieldCropPest");

            migrationBuilder.DropColumn(
                name: "FieldCropId",
                table: "FieldCropPest");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "FieldCropPest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPest_FieldId_CropPestId",
                table: "FieldCropPest",
                columns: new[] { "FieldId", "CropPestId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CropPest_Field",
                table: "FieldCropPest",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
