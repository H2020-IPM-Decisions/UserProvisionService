using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ChangeWeatherDataSourceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWeatherDataSource");

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

            migrationBuilder.AddColumn<bool>(
                name: "AuthenticationRequired",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Credentials",
                table: "WeatherDataSource",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FarmId",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsForecast",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StationId",
                table: "WeatherDataSource",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnd",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStart",
                table: "WeatherDataSource",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_WeatherDataSource_FarmId",
                table: "WeatherDataSource",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWeatherDataSource_Farm",
                table: "WeatherDataSource",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWeatherDataSource_Farm",
                table: "WeatherDataSource");

            migrationBuilder.DropIndex(
                name: "IX_WeatherDataSource_FarmId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "AuthenticationRequired",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Credentials",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "IsForecast",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "TimeEnd",
                table: "WeatherDataSource");

            migrationBuilder.DropColumn(
                name: "TimeStart",
                table: "WeatherDataSource");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "WeatherDataSource",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "WeatherDataSource",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "WeatherDataSource",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FileUploadedOn",
                table: "WeatherDataSource",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FarmWeatherDataSource",
                columns: table => new
                {
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherDataSourceId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWeatherDataSource", x => new { x.FarmId, x.WeatherDataSourceId });
                    table.ForeignKey(
                        name: "FK_FarmWeatherDataSource_Farm",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmWeatherDataSource_WeatherDataSource",
                        column: x => x.WeatherDataSourceId,
                        principalTable: "WeatherDataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherDataSource_WeatherDataSourceId",
                table: "FarmWeatherDataSource",
                column: "WeatherDataSourceId");
        }
    }
}
