using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class TableWeatherForecastChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldWeatherDataSource");

            migrationBuilder.DropTable(
                name: "FieldWeatherStation");

            migrationBuilder.DropTable(
                name: "WeatherDataSource");

            migrationBuilder.CreateTable(
                name: "WeatherForecast",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeatherId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecast", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmWeatherForecast",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(nullable: false),
                    WeatherForecastId = table.Column<Guid>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecast_WeatherId",
                table: "WeatherForecast",
                column: "WeatherId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWeatherForecast");

            migrationBuilder.DropTable(
                name: "WeatherForecast");

            migrationBuilder.CreateTable(
                name: "FieldWeatherStation",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherStationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldWeatherStation", x => new { x.FieldId, x.WeatherStationId });
                    table.ForeignKey(
                        name: "FK_FieldWeatherStation_Field",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldWeatherStation_WeatherStation",
                        column: x => x.WeatherStationId,
                        principalTable: "WeatherStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherDataSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthenticationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Credentials = table.Column<string>(type: "jsonb", nullable: true),
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    IsForecast = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TimeEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDataSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmWeatherDataSource_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldWeatherDataSource",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherDataSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldWeatherDataSource", x => x.FieldId);
                    table.ForeignKey(
                        name: "FK_FieldWeatherDataSource_Field",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldWeatherDataSource_WeatherDataSource",
                        column: x => x.WeatherDataSourceId,
                        principalTable: "WeatherDataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherStation_WeatherStationId",
                table: "FieldWeatherStation",
                column: "WeatherStationId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherDataSource_FarmId",
                table: "WeatherDataSource",
                column: "FarmId");
        }
    }
}
