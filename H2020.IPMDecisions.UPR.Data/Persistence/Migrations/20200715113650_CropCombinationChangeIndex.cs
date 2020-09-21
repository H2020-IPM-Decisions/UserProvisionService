using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class CropCombinationChangeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CropDecisionCombination",
                table: "CropDecisionCombination");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CropDecisionCombination",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CropDecisionCombination",
                table: "CropDecisionCombination",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_CropId_DssId_PestId",
                table: "CropDecisionCombination",
                columns: new[] { "CropId", "DssId", "PestId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CropDecisionCombination",
                table: "CropDecisionCombination");

            migrationBuilder.DropIndex(
                name: "IX_CropDecisionCombination_CropId_DssId_PestId",
                table: "CropDecisionCombination");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CropDecisionCombination");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CropDecisionCombination",
                table: "CropDecisionCombination",
                columns: new[] { "CropId", "DssId", "PestId" });
        }
    }
}
