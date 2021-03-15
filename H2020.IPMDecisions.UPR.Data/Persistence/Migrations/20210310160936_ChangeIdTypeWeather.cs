using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ChangeIdTypeWeather : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WeatherDataSource");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WeatherDataSource",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherDataSource",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempId",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource",
                column: "TempId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId",
                principalTable: "WeatherDataSource",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "NewId",
                table: "WeatherDataSource");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WeatherDataSource",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherDataSource",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "WeatherDataSource",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherDataSource",
                table: "WeatherDataSource",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId",
                principalTable: "WeatherDataSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
