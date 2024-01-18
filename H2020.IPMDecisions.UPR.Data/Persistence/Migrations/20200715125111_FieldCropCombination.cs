using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class FieldCropCombination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldCropDecisionCombination",
                columns: table => new
                {
                    FielId = table.Column<Guid>(nullable: false),
                    CropDecisionCombinationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCropDecisionCombination", x => new { x.FielId, x.CropDecisionCombinationId });
                    table.ForeignKey(
                        name: "FK_FieldCropDecision_CropDecision",
                        column: x => x.CropDecisionCombinationId,
                        principalTable: "CropDecisionCombination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldCropDecision_Field",
                        column: x => x.FielId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropDecisionCombination_CropDecisionCombinationId",
                table: "FieldCropDecisionCombination",
                column: "CropDecisionCombinationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldCropDecisionCombination");
        }
    }
}
