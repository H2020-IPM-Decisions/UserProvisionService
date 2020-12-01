using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveIndexFromFarmLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Farm_Location",
                table: "Farm");

            migrationBuilder.CreateIndex(
                name: "IX_Farm_Location",
                table: "Farm",
                column: "Location");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Farm_Location",
                table: "Farm");

            migrationBuilder.CreateIndex(
                name: "IX_Farm_Location",
                table: "Farm",
                column: "Location",
                unique: true);
        }
    }
}
