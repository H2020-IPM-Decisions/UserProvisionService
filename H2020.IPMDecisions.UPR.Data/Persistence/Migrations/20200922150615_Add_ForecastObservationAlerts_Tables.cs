using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class Add_ForecastObservationAlerts_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldCropDecisionCombination");

            migrationBuilder.CreateTable(
                name: "ForecastAlert",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeatherStationId = table.Column<Guid>(nullable: false),
                    CropPestDssCombinationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastAlert_CropPestDssCombination_Id",
                        column: x => x.CropPestDssCombinationId,
                        principalTable: "CropPestDssCombination",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObservationAlert",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeatherStationId = table.Column<Guid>(nullable: false),
                    CropPestDssCombinationId = table.Column<Guid>(nullable: false),
                    FieldObservationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationAlert_CropPestDssCombination_Id",
                        column: x => x.CropPestDssCombinationId,
                        principalTable: "CropPestDssCombination",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObservationAlert_FieldObservation_FieldObservationId",
                        column: x => x.FieldObservationId,
                        principalTable: "FieldObservation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForecastResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ForecastAlertId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Inf1 = table.Column<string>(nullable: true),
                    Inf2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastResult_ForecastAlert_ForecastAlertId",
                        column: x => x.ForecastAlertId,
                        principalTable: "ForecastAlert",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObservationResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ObservationAlertId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Inf1 = table.Column<string>(nullable: true),
                    Inf2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationResult_ObservationAlert_ObservationAlertId",
                        column: x => x.ObservationAlertId,
                        principalTable: "ObservationAlert",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastAlert_CropPestDssCombinationId",
                table: "ForecastAlert",
                column: "CropPestDssCombinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastAlert_WeatherStationId_CropPestDssCombinationId",
                table: "ForecastAlert",
                columns: new[] { "WeatherStationId", "CropPestDssCombinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResult_ForecastAlertId_Date",
                table: "ForecastResult",
                columns: new[] { "ForecastAlertId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_CropPestDssCombinationId",
                table: "ObservationAlert",
                column: "CropPestDssCombinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_FieldObservationId",
                table: "ObservationAlert",
                column: "FieldObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_WeatherStationId_CropPestDssCombinationId_~",
                table: "ObservationAlert",
                columns: new[] { "WeatherStationId", "CropPestDssCombinationId", "FieldObservationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObservationResult_ObservationAlertId",
                table: "ObservationResult",
                column: "ObservationAlertId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForecastResult");

            migrationBuilder.DropTable(
                name: "ObservationResult");

            migrationBuilder.DropTable(
                name: "ForecastAlert");

            migrationBuilder.DropTable(
                name: "ObservationAlert");

            migrationBuilder.CreateTable(
                name: "FieldCropDecisionCombination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCropDecisionCombination", x => x.Id);
                });
        }
    }
}
