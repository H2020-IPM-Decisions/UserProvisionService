using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class MoveObservationToCropPest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_Field",
                table: "FieldObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldObservation_FieldId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "CropEppoCode",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "PestEppoCode",
                table: "FieldObservation");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropPestdId",
                table: "FieldObservation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldObservation_FieldCropPestdId",
                table: "FieldObservation",
                column: "FieldCropPestdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation",
                column: "FieldCropPestdId",
                principalTable: "FieldCropPest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldObservation_FieldCropPestdId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "FieldCropPestdId",
                table: "FieldObservation");

            migrationBuilder.AddColumn<string>(
                name: "CropEppoCode",
                table: "FieldObservation",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "FieldObservation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PestEppoCode",
                table: "FieldObservation",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FieldObservation_FieldId",
                table: "FieldObservation",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_Field",
                table: "FieldObservation",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
