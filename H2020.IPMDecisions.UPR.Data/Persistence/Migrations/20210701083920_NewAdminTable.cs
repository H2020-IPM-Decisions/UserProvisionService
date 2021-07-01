using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class NewAdminTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdministrationVariable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrationVariable", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AdministrationVariable",
                columns: new[] { "Id", "Description", "Value" },
                values: new object[,]
                {
                    { 0, "WeatherForecastService", null },
                    { 1, "WeatherHistoricalService", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrationVariable_Description",
                table: "AdministrationVariable",
                column: "Description",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrationVariable");
        }
    }
}
