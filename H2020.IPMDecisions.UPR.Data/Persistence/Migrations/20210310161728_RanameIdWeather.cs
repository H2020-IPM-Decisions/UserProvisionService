using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RanameIdWeather : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldWeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "NewId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "WeatherDataSourceId",
                table: "FieldWeatherDataSource");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldWeatherDataSource",
                table: "FieldWeatherDataSource",
                column: "FieldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldWeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WeatherDataSource");

            migrationBuilder.AddColumn<string>(
                name: "TempId",
                table: "WeatherDataSource",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "WeatherDataSource",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource",
                column: "TempId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldWeatherDataSource",
                table: "FieldWeatherDataSource",
                columns: new[] { "FieldId", "WeatherDataSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId",
                principalTable: "WeatherDataSource",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
