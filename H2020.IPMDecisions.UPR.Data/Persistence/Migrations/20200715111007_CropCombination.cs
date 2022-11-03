using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class CropCombination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crop",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CropDecisionCombination",
                columns: table => new
                {
                    CropId = table.Column<Guid>(nullable: false),
                    DssId = table.Column<Guid>(nullable: false),
                    PestId = table.Column<Guid>(nullable: false),
                    Inf1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropDecisionCombination", x => new { x.CropId, x.DssId, x.PestId });
                    table.ForeignKey(
                        name: "FK_CropCombination_Crop",
                        column: x => x.CropId,
                        principalTable: "Crop",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CropCombination_Dss",
                        column: x => x.DssId,
                        principalTable: "Dss",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CropCombination_Pest",
                        column: x => x.PestId,
                        principalTable: "Pest",
                        principalColumn: "Id");
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CropDecisionCombination");

            migrationBuilder.DropTable(
                name: "Crop");

            migrationBuilder.DropTable(
                name: "Dss");

            migrationBuilder.DropTable(
                name: "Pest");
        }
    }
}
