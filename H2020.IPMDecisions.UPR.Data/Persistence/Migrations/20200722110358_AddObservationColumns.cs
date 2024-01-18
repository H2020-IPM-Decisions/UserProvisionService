using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddObservationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "Inf3",
                table: "FieldObservation");

            migrationBuilder.AddColumn<string>(
                name: "CropEppoCode",
                table: "FieldObservation",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "FieldObservation",
                type: "geometry (point)",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "PestEppoCode",
                table: "FieldObservation",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "FieldObservation",
                nullable: false,
                defaultValueSql: "NOW()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CropEppoCode",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "PestEppoCode",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "FieldObservation");

            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "FieldObservation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "FieldObservation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf3",
                table: "FieldObservation",
                type: "text",
                nullable: true);
        }
    }
}
