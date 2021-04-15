using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class WeatherForcastOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWeatherForecast");

            migrationBuilder.AddColumn<Guid>(
                name: "WeatherForecastId",
                table: "Farm",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Farm_WeatherForecastId",
                table: "Farm",
                column: "WeatherForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm",
                column: "WeatherForecastId",
                principalTable: "WeatherForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm");

            migrationBuilder.DropIndex(
                name: "IX_Farm_WeatherForecastId",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "WeatherForecastId",
                table: "Farm");

            migrationBuilder.CreateTable(
                name: "FarmWeatherForecast",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherForecastId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWeatherForecast", x => new { x.FarmId, x.WeatherForecastId });
                    table.ForeignKey(
                        name: "FK_FarmWeatherForecast_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmWeatherForecast_WeatherForecast_WeatherForecastId",
                        column: x => x.WeatherForecastId,
                        principalTable: "WeatherForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherForecast_WeatherForecastId",
                table: "FarmWeatherForecast",
                column: "WeatherForecastId");
        }
    }
}
