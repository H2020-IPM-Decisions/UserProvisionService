using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class CropPestDssResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CropPestDssResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CropPestDssId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Inf1 = table.Column<string>(nullable: true),
                    Inf2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPestDssResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropPestDss_CropPestDssResult",
                        column: x => x.CropPestDssId,
                        principalTable: "CropPestDss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDssResult_CropPestDssId",
                table: "CropPestDssResult",
                column: "CropPestDssId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CropPestDssResult");
        }
    }
}
