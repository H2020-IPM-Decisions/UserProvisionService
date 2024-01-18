using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveTempColum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssParameters",
                table: "FieldCropPestDss");

            migrationBuilder.CreateTable(
                name: "FieldDssResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    FieldCropPestDssId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDssResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId",
                        column: x => x.FieldCropPestDssId,
                        principalTable: "FieldCropPestDss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldDssResult_FieldCropPestDssId",
                table: "FieldDssResult",
                column: "FieldCropPestDssId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldDssResult");

            migrationBuilder.AddColumn<string>(
                name: "DssParameters",
                table: "FieldCropPestDss",
                type: "text",
                nullable: true);
        }
    }
}
