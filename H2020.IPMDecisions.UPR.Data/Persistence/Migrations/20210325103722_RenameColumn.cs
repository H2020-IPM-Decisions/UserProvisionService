using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RenameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CredentialsTempJson",
                table: "WeatherStation",
                newName: "Credentials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Credentials",
                table: "WeatherStation",
                newName: "CredentialsTempJson");
        }
    }
}
