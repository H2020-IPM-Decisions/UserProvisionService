using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveFarmPlaceholders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inf1",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "Inf2",
                table: "Farm");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Inf1",
                table: "Farm",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inf2",
                table: "Farm",
                type: "text",
                nullable: true);
        }
    }
}
