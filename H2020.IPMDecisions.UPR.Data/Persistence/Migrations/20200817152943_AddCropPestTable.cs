using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddCropPestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CropCombination_Crop",
                table: "CropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_CropCombination_Dss",
                table: "CropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_CropCombination_Pest",
                table: "CropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldCropDecision_CropDecision",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldCropDecision_Field",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldCropDecisionCombination",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropIndex(
                name: "IX_FieldCropDecisionCombination_CropDecisionCombinationId",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropIndex(
                name: "IX_CropDecisionCombination_CropId_DssId_PestId",
                table: "CropDecisionCombination");

            migrationBuilder.DropColumn(
                name: "FielId",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropColumn(
                name: "CropDecisionCombinationId",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "CropDecisionCombination");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FieldCropDecisionCombination",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "PestId",
                table: "CropDecisionCombination",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DssId",
                table: "CropDecisionCombination",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "CropId",
                table: "CropDecisionCombination",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldCropDecisionCombination",
                table: "FieldCropDecisionCombination",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CropPest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CropEppoCode = table.Column<string>(maxLength: 6, nullable: false),
                    PestEppoCode = table.Column<string>(maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldCropPest",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(nullable: false),
                    CropPestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCropPest", x => new { x.FieldId, x.CropPestId });
                    table.ForeignKey(
                        name: "FK_CropPest_Crop",
                        column: x => x.CropPestId,
                        principalTable: "CropPest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CropPest_Field",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_Crop",
                table: "CropDecisionCombination",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CropPest_CropEppoCode_PestEppoCode",
                table: "CropPest",
                columns: new[] { "CropEppoCode", "PestEppoCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropPest_CropPestId",
                table: "FieldCropPest",
                column: "CropPestId");

            migrationBuilder.AddForeignKey(
                name: "FK_CropDecisionCombination_Crop_CropId",
                table: "CropDecisionCombination",
                column: "CropId",
                principalTable: "Crop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CropDecisionCombination_Dss_DssId",
                table: "CropDecisionCombination",
                column: "DssId",
                principalTable: "Dss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CropDecisionCombination_Pest_PestId",
                table: "CropDecisionCombination",
                column: "PestId",
                principalTable: "Pest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CropDecisionCombination_Crop_CropId",
                table: "CropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_CropDecisionCombination_Dss_DssId",
                table: "CropDecisionCombination");

            migrationBuilder.DropForeignKey(
                name: "FK_CropDecisionCombination_Pest_PestId",
                table: "CropDecisionCombination");

            migrationBuilder.DropTable(
                name: "FieldCropPest");

            migrationBuilder.DropTable(
                name: "CropPest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldCropDecisionCombination",
                table: "FieldCropDecisionCombination");

            migrationBuilder.DropIndex(
                name: "IX_CropDecisionCombination_CropId",
                table: "CropDecisionCombination");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FieldCropDecisionCombination");

            migrationBuilder.AddColumn<Guid>(
                name: "FielId",
                table: "FieldCropDecisionCombination",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CropDecisionCombinationId",
                table: "FieldCropDecisionCombination",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "PestId",
                table: "CropDecisionCombination",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DssId",
                table: "CropDecisionCombination",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CropId",
                table: "CropDecisionCombination",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "CropDecisionCombination",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldCropDecisionCombination",
                table: "FieldCropDecisionCombination",
                columns: new[] { "FielId", "CropDecisionCombinationId" });

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropDecisionCombination_CropDecisionCombinationId",
                table: "FieldCropDecisionCombination",
                column: "CropDecisionCombinationId");

            migrationBuilder.CreateIndex(
                name: "IX_CropDecisionCombination_CropId_DssId_PestId",
                table: "CropDecisionCombination",
                columns: new[] { "CropId", "DssId", "PestId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CropCombination_Crop",
                table: "CropDecisionCombination",
                column: "CropId",
                principalTable: "Crop",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CropCombination_Dss",
                table: "CropDecisionCombination",
                column: "DssId",
                principalTable: "Dss",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CropCombination_Pest",
                table: "CropDecisionCombination",
                column: "PestId",
                principalTable: "Pest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldCropDecision_CropDecision",
                table: "FieldCropDecisionCombination",
                column: "CropDecisionCombinationId",
                principalTable: "CropDecisionCombination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldCropDecision_Field",
                table: "FieldCropDecisionCombination",
                column: "FielId",
                principalTable: "Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
