using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class fieldObersvations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Inf1 = table.Column<string>(nullable: false),
                    Inf2 = table.Column<string>(nullable: true),
                    Inf3 = table.Column<string>(nullable: true),
                    FieldId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observation_Field",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldObservation_FieldId",
                table: "FieldObservation",
                column: "FieldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldObservation");
        }
    }
}
