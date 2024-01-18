using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class RemoveRequiredLocationFromObservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "FieldObservation",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry (point)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "FieldObservation",
                type: "geometry (point)",
                nullable: false,
                oldClrType: typeof(Point),
                oldNullable: true);
        }
    }
}
