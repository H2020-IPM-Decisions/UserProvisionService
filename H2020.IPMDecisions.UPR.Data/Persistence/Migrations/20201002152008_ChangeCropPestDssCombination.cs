using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class ChangeCropPestDssCombination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForecastAlert_CropPestDssCombination_Id",
                table: "ForecastAlert");

            migrationBuilder.DropForeignKey(
                name: "FK_ForecastResult_ForecastAlert_ForecastAlertId",
                table: "ForecastResult");

            migrationBuilder.DropForeignKey(
                name: "FK_ObservationAlert_CropPestDssCombination_Id",
                table: "ObservationAlert");

            migrationBuilder.DropForeignKey(
                name: "FK_ObservationResult_ObservationAlert_ObservationAlertId",
                table: "ObservationResult");

            migrationBuilder.DropTable(
                name: "CropPestDssCombination");

            migrationBuilder.DropIndex(
                name: "IX_ObservationAlert_CropPestDssCombinationId",
                table: "ObservationAlert");

            migrationBuilder.DropIndex(
                name: "IX_ObservationAlert_WeatherStationId_CropPestDssCombinationId_~",
                table: "ObservationAlert");

            migrationBuilder.DropIndex(
                name: "IX_ForecastAlert_CropPestDssCombinationId",
                table: "ForecastAlert");

            migrationBuilder.DropIndex(
                name: "IX_ForecastAlert_WeatherStationId_CropPestDssCombinationId",
                table: "ForecastAlert");

            migrationBuilder.DropColumn(
                name: "CropPestDssCombinationId",
                table: "ObservationAlert");

            migrationBuilder.DropColumn(
                name: "CropPestDssCombinationId",
                table: "ForecastAlert");

            migrationBuilder.CreateTable(
                name: "CropPestDss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CropPestId = table.Column<Guid>(nullable: false),
                    DssId = table.Column<string>(nullable: false),
                    DssName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPestDss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropPest_CropPestDss",
                        column: x => x.CropPestId,
                        principalTable: "CropPest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_WeatherStationId_FieldObservationId",
                table: "ObservationAlert",
                columns: new[] { "WeatherStationId", "FieldObservationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CropPestDss_CropPestId_DssName",
                table: "CropPestDss",
                columns: new[] { "CropPestId", "DssName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ForecastResult_ForecastAlert_ForecastAlertId",
                table: "ForecastResult",
                column: "ForecastAlertId",
                principalTable: "ForecastAlert",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationResult_ObservationAlert_ObservationAlertId",
                table: "ObservationResult",
                column: "ObservationAlertId",
                principalTable: "ObservationAlert",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForecastResult_ForecastAlert_ForecastAlertId",
                table: "ForecastResult");

            migrationBuilder.DropForeignKey(
                name: "FK_ObservationResult_ObservationAlert_ObservationAlertId",
                table: "ObservationResult");

            migrationBuilder.DropTable(
                name: "CropPestDss");

            migrationBuilder.DropIndex(
                name: "IX_ObservationAlert_WeatherStationId_FieldObservationId",
                table: "ObservationAlert");

            migrationBuilder.AddColumn<Guid>(
                name: "CropPestDssCombinationId",
                table: "ObservationAlert",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CropPestDssCombinationId",
                table: "ForecastAlert",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CropPestDssCombination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropPestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DssName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPestDssCombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropPestDss_Combination",
                        column: x => x.CropPestId,
                        principalTable: "CropPest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_CropPestDssCombinationId",
                table: "ObservationAlert",
                column: "CropPestDssCombinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAlert_WeatherStationId_CropPestDssCombinationId_~",
                table: "ObservationAlert",
                columns: new[] { "WeatherStationId", "CropPestDssCombinationId", "FieldObservationId" },
                unique: true);

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
                name: "IX_CropPestDssCombination_CropPestId_DssName",
                table: "CropPestDssCombination",
                columns: new[] { "CropPestId", "DssName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ForecastAlert_CropPestDssCombination_Id",
                table: "ForecastAlert",
                column: "CropPestDssCombinationId",
                principalTable: "CropPestDssCombination",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForecastResult_ForecastAlert_ForecastAlertId",
                table: "ForecastResult",
                column: "ForecastAlertId",
                principalTable: "ForecastAlert",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationAlert_CropPestDssCombination_Id",
                table: "ObservationAlert",
                column: "CropPestDssCombinationId",
                principalTable: "CropPestDssCombination",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationResult_ObservationAlert_ObservationAlertId",
                table: "ObservationResult",
                column: "ObservationAlertId",
                principalTable: "ObservationAlert",
                principalColumn: "Id");
        }
    }
}
