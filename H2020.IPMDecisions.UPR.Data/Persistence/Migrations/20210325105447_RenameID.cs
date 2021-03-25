using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RenameID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherStation_WeatherStation_WeatherStationTempId",
                table: "FieldWeatherStation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherStation",
                table: "WeatherStation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldWeatherStation",
                table: "FieldWeatherStation");

            migrationBuilder.DropIndex(
                name: "IX_FieldWeatherStation_WeatherStationTempId",
                table: "FieldWeatherStation");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "WeatherStationTempId",
                table: "FieldWeatherStation");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WeatherStation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WeatherStationId",
                table: "FieldWeatherStation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherStation",
                table: "WeatherStation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldWeatherStation",
                table: "FieldWeatherStation",
                columns: new[] { "FieldId", "WeatherStationId" });

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherStation_WeatherStationId",
                table: "FieldWeatherStation",
                column: "WeatherStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherStation_WeatherStation",
                table: "FieldWeatherStation",
                column: "WeatherStationId",
                principalTable: "WeatherStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherStation_WeatherStation",
                table: "FieldWeatherStation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherStation",
                table: "WeatherStation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldWeatherStation",
                table: "FieldWeatherStation");

            migrationBuilder.DropIndex(
                name: "IX_FieldWeatherStation_WeatherStationId",
                table: "FieldWeatherStation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "WeatherStationId",
                table: "FieldWeatherStation");

            migrationBuilder.AddColumn<Guid>(
                name: "TempId",
                table: "WeatherStation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WeatherStationTempId",
                table: "FieldWeatherStation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherStation",
                table: "WeatherStation",
                column: "TempId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldWeatherStation",
                table: "FieldWeatherStation",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherStation_WeatherStationTempId",
                table: "FieldWeatherStation",
                column: "WeatherStationTempId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherStation_WeatherStation_WeatherStationTempId",
                table: "FieldWeatherStation",
                column: "WeatherStationTempId",
                principalTable: "WeatherStation",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
