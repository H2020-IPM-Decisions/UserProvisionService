using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AllowFilesWeatherDataSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FileUploadedOn",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "WeatherDataSource",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "FileUploadedOn",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "WeatherDataSource");
        }
    }
}
