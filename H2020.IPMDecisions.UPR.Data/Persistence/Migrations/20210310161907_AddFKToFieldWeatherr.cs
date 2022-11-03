using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddFKToFieldWeatherr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId",
                principalTable: "WeatherDataSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldWeatherDataSource_WeatherDataSource",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource");

            migrationBuilder.DropColumn(
                name: "WeatherDataSourceId",
                table: "FieldWeatherDataSource");
        }
    }
}
