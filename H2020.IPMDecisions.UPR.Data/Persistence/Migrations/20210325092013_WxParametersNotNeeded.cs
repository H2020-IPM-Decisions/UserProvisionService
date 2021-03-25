using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class WxParametersNotNeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "WeatherStation");

            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "WeatherDataSource");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "WeatherStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "WeatherDataSource",
                type: "text",
                nullable: true);
        }
    }
}
