using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class Revert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldDssObservation_FieldObservation_FieldObservationId",
                table: "FieldDssObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldDssObservation_FieldObservationId",
                table: "FieldDssObservation");

            migrationBuilder.DropColumn(
                name: "FieldObservationId",
                table: "FieldDssObservation");

            migrationBuilder.AddColumn<string>(
                name: "DssObservation",
                table: "FieldObservation",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DssObservation",
                table: "FieldDssObservation",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "FieldDssObservation",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "FieldDssObservation",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDssObservation_FieldCropPestDssId",
                table: "FieldDssObservation",
                column: "FieldCropPestDssId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_FieldCropPestDss",
                table: "FieldDssObservation",
                column: "FieldCropPestDssId",
                principalTable: "FieldCropPestDss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_FieldCropPestDss",
                table: "FieldDssObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldDssObservation_FieldCropPestDssId",
                table: "FieldDssObservation");

            migrationBuilder.DropColumn(
                name: "DssObservation",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "FieldDssObservation");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "FieldDssObservation");

            migrationBuilder.AlterColumn<string>(
                name: "DssObservation",
                table: "FieldDssObservation",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FieldObservationId",
                table: "FieldDssObservation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldDssObservation_FieldObservationId",
                table: "FieldDssObservation",
                column: "FieldObservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldDssObservation_FieldObservation_FieldObservationId",
                table: "FieldDssObservation",
                column: "FieldObservationId",
                principalTable: "FieldObservation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
