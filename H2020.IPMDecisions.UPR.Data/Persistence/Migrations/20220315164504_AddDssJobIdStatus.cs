using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddDssJobIdStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastJobId",
                table: "FieldCropPestDss",
                nullable: true);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Field",
                type: "geometry (point)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastJobId",
                table: "FieldCropPestDss");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Field",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry (point)",
                oldNullable: true);
        }
    }
}
