using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddSprayApplicationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldSprayApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "NOW()"),
                    Rate = table.Column<double>(nullable: false),
                    FieldCropPestdId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldSprayApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spray_FieldCropPest",
                        column: x => x.FieldCropPestdId,
                        principalTable: "FieldCropPest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldSprayApplication_FieldCropPestdId",
                table: "FieldSprayApplication",
                column: "FieldCropPestdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldSprayApplication");
        }
    }
}
