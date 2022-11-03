using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class FieldCropPestDss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DssId",
                table: "CropPestDss",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FieldCropPestDss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FieldCropPestId = table.Column<Guid>(nullable: false),
                    CropPestDssId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCropPestDss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldCropPestDss_CropPestDss",
                        column: x => x.CropPestDssId,
                        principalTable: "CropPestDss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldCropPestDss_FieldCropPest_FieldCropPestId",
                        column: x => x.FieldCropPestId,
                        principalTable: "FieldCropPest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPestDss_CropPestDssId",
                table: "FieldCropPestDss",
                column: "CropPestDssId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId",
                table: "FieldCropPestDss",
                columns: new[] { "FieldCropPestId", "CropPestDssId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldCropPestDss");

            migrationBuilder.AlterColumn<string>(
                name: "DssId",
                table: "CropPestDss",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
