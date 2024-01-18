using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddWeatherToField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldWeatherDataSource",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(nullable: false),
                    WeatherDataSourceId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldWeatherDataSource", x => new { x.FieldId, x.WeatherDataSourceId });
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

            migrationBuilder.CreateTable(
                name: "FieldWeatherStation",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(nullable: false),
                    WeatherStationId = table.Column<string>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherDataSource_WeatherDataSourceId",
                table: "FieldWeatherDataSource",
                column: "WeatherDataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldWeatherStation_WeatherStationId",
                table: "FieldWeatherStation",
                column: "WeatherStationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldWeatherDataSource");

            migrationBuilder.DropTable(
                name: "FieldWeatherStation");
        }
    }
}
