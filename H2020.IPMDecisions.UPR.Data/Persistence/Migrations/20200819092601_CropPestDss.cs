using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class CropPestDss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CropDecisionCombination");

            migrationBuilder.DropTable(
                name: "Crop");

            migrationBuilder.DropTable(
                name: "Dss");

            migrationBuilder.DropTable(
                name: "Pest");

            migrationBuilder.CreateTable(
                name: "CropPestDssCombination",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CropPestId = table.Column<Guid>(nullable: false),
                    DssName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPestDssCombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropPestDss_Combination",
                        column: x => x.CropPestId,
                        principalTable: "CropPest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDssCombination_CropPestId_DssName",
                table: "CropPestDssCombination",
                columns: new[] { "CropPestId", "DssName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CropPestDssCombination");

            migrationBuilder.CreateTable(
                name: "Crop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CropDecisionCombination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    DssId = table.Column<Guid>(type: "uuid", nullable: true),
                    PestId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropDecisionCombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropDecisionCombination_Crop_CropId",
                        column: x => x.CropId,
                        principalTable: "Crop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CropDecisionCombination_Dss_DssId",
                        column: x => x.DssId,
                        principalTable: "Dss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CropDecisionCombination_Pest_PestId",
                        column: x => x.PestId,
                        principalTable: "Pest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_CropId",
                table: "CropDecisionCombination",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_DssId",
                table: "CropDecisionCombination",
                column: "DssId");

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_PestId",
                table: "CropDecisionCombination",
                column: "PestId");
        }
    }
}
