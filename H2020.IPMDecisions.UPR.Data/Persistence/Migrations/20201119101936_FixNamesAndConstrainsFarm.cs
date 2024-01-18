using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class FixNamesAndConstrainsFarm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWeatherDataSource_WeatherDataSource",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmWeatherDataSource",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropIndex(
                name: "IX_FarmWeatherDataSource_WeatherForecastServiceId",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropColumn(
                name: "WeatherForecastServiceId",
                table: "FarmWeatherDataSource");

            migrationBuilder.AddColumn<string>(
                name: "WeatherDataSourceId",
                table: "FarmWeatherDataSource",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmWeatherDataSource",
                table: "FarmWeatherDataSource",
                columns: new[] { "FarmId", "WeatherDataSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherDataSource_WeatherDataSourceId",
                table: "FarmWeatherDataSource",
                column: "WeatherDataSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWeatherDataSource_WeatherDataSource",
                table: "FarmWeatherDataSource",
                column: "WeatherDataSourceId",
                principalTable: "WeatherDataSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWeatherDataSource_WeatherDataSource",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmWeatherDataSource",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropIndex(
                name: "IX_FarmWeatherDataSource_WeatherDataSourceId",
                table: "FarmWeatherDataSource");

            migrationBuilder.DropColumn(
                name: "WeatherDataSourceId",
                table: "FarmWeatherDataSource");

            migrationBuilder.AddColumn<string>(
                name: "WeatherForecastServiceId",
                table: "FarmWeatherDataSource",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmWeatherDataSource",
                table: "FarmWeatherDataSource",
                columns: new[] { "FarmId", "WeatherForecastServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_FarmWeatherDataSource_WeatherForecastServiceId",
                table: "FarmWeatherDataSource",
                column: "WeatherForecastServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWeatherDataSource_WeatherDataSource",
                table: "FarmWeatherDataSource",
                column: "WeatherForecastServiceId",
                principalTable: "WeatherDataSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
