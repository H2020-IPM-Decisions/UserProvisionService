using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddLocationToField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WarningStatus",
                table: "FieldDssResult",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Field",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Field");

            migrationBuilder.AlterColumn<int>(
                name: "WarningStatus",
                table: "FieldDssResult",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
