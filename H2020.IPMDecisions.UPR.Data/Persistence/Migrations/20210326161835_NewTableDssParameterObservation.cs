using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class NewTableDssParameterObservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldObservation_FieldCropPestdId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "FieldCropPestdId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "ObservationProperties",
                table: "FieldCropPestDss");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropPestId",
                table: "FieldObservation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "FieldDssObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Location = table.Column<Point>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "NOW()"),
                    FieldCropPestDssId = table.Column<Guid>(nullable: false),
                    DssObservation = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDssObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observation_FieldCropPestDss",
                        column: x => x.FieldCropPestDssId,
                        principalTable: "FieldCropPestDss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldObservation_FieldCropPestId",
                table: "FieldObservation",
                column: "FieldCropPestId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDssObservation_FieldCropPestDssId",
                table: "FieldDssObservation",
                column: "FieldCropPestDssId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation",
                column: "FieldCropPestId",
                principalTable: "FieldCropPest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation");

            migrationBuilder.DropTable(
                name: "FieldDssObservation");

            migrationBuilder.DropIndex(
                name: "IX_FieldObservation_FieldCropPestId",
                table: "FieldObservation");

            migrationBuilder.DropColumn(
                name: "FieldCropPestId",
                table: "FieldObservation");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldCropPestdId",
                table: "FieldObservation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ObservationProperties",
                table: "FieldCropPestDss",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldObservation_FieldCropPestdId",
                table: "FieldObservation",
                column: "FieldCropPestdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_FieldCropPest",
                table: "FieldObservation",
                column: "FieldCropPestdId",
                principalTable: "FieldCropPest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
