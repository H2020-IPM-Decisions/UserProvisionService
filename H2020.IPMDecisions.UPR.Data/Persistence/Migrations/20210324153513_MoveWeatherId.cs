using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class MoveWeatherId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWeatherStation");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherStation",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AuthenticationRequired",
                table: "WeatherStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Credentials",
                table: "WeatherStation",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FarmId",
                table: "WeatherStation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "WeatherStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsForecast",
                table: "WeatherStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "WeatherStation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StationId",
                table: "WeatherStation",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnd",
                table: "WeatherStation",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStart",
                table: "WeatherStation",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "WeatherStation",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherStation_FarmId",
                table: "WeatherStation",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_FarmWeatherStations",
                table: "WeatherStation",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_FarmWeatherStations",
                table: "WeatherStation");

            migrationBuilder.DropIndex(
                name: "IX_WeatherStation_FarmId",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "AuthenticationRequired",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Credentials",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "IsForecast",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "TimeEnd",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "TimeStart",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "WeatherStation");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherStation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "FarmWeatherStation",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherStationId = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_FarmWeatherStation_WeatherStationId",
                table: "FarmWeatherStation",
                column: "WeatherStationId");
        }
    }
}
