using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddFluentAPIToWxStations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_FarmWeatherStations",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Credentials",
                table: "WeatherStation");

            migrationBuilder.AddColumn<string>(
                name: "CredentialsTempJson",
                table: "WeatherStation",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWeatherStation_Farm",
                table: "WeatherStation",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWeatherStation_Farm",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "CredentialsTempJson",
                table: "WeatherStation");

            migrationBuilder.AddColumn<string>(
                name: "Credentials",
                table: "WeatherStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_FarmWeatherStations",
                table: "WeatherStation",
                column: "FarmId",
                principalTable: "Farm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
