using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class WeatherHistoricalOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm");

            migrationBuilder.DropTable(
                name: "WeatherStation");

            migrationBuilder.AddColumn<Guid>(
                name: "WeatherHistoricalId",
                table: "Farm",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "WeatherHistorical",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeatherId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherHistorical", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farm_WeatherHistoricalId",
                table: "Farm",
                column: "WeatherHistoricalId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherHistorical_WeatherId",
                table: "WeatherHistorical",
                column: "WeatherId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm",
                column: "WeatherForecastId",
                principalTable: "WeatherForecast",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_WeatherHistorical",
                table: "Farm",
                column: "WeatherHistoricalId",
                principalTable: "WeatherHistorical",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm");

            migrationBuilder.DropForeignKey(
                name: "FK_Farm_WeatherHistorical",
                table: "Farm");

            migrationBuilder.DropTable(
                name: "WeatherHistorical");

            migrationBuilder.DropIndex(
                name: "IX_Farm_WeatherHistoricalId",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "WeatherHistoricalId",
                table: "Farm");

            migrationBuilder.CreateTable(
                name: "WeatherStation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthenticationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Credentials = table.Column<string>(type: "jsonb", nullable: true),
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    IsForecast = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StationId = table.Column<string>(type: "text", nullable: true),
                    TimeEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmWeatherStation_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherStation_FarmId",
                table: "WeatherStation",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_WeatherForecast",
                table: "Farm",
                column: "WeatherForecastId",
                principalTable: "WeatherForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
