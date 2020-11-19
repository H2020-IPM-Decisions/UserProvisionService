using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class WeatherStationAndSourceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherDataSource",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDataSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherStation",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmWeatherDataSource",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(nullable: false),
                    WeatherForecastServiceId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWeatherDataSource", x => new { x.FarmId, x.WeatherForecastServiceId });
                    table.ForeignKey(
                        name: "FK_FarmWeatherDataSource_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmWeatherDataSource_WeatherDataSource",
                        column: x => x.WeatherForecastServiceId,
                        principalTable: "WeatherDataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FarmWeatherStation",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(nullable: false),
                    WeatherStationId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWeatherStation", x => new { x.FarmId, x.WeatherStationId });
                    table.ForeignKey(
                        name: "FK_FarmWeatherStation_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmWeatherStation_WeatherStation",
                        column: x => x.WeatherStationId,
                        principalTable: "WeatherStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherDataSource_WeatherForecastServiceId",
                table: "FarmWeatherDataSource",
                column: "WeatherForecastServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherStation_WeatherStationId",
                table: "FarmWeatherStation",
                column: "WeatherStationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWeatherDataSource");

            migrationBuilder.DropTable(
                name: "FarmWeatherStation");

            migrationBuilder.DropTable(
                name: "WeatherDataSource");

            migrationBuilder.DropTable(
                name: "WeatherStation");
        }
    }
}
