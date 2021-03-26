using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class DssParameterObservitionOnObservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DssObservation",
                table: "FieldObservation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssObservation",
                table: "FieldObservation");
        }
    }
}
