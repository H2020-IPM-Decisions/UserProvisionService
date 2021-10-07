using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class DssVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DssResult");

            migrationBuilder.AddColumn<string>(
                name: "DssVersion",
                table: "CropPestDss",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DssVersion",
                table: "CropPestDss");

            migrationBuilder.CreateTable(
                name: "DssResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CropEppoCode = table.Column<string>(type: "text", nullable: true),
                    DssExecutionType = table.Column<string>(type: "text", nullable: true),
                    DssFullResult = table.Column<string>(type: "text", nullable: true),
                    DssId = table.Column<string>(type: "text", nullable: true),
                    DssModelId = table.Column<string>(type: "text", nullable: true),
                    DssModelName = table.Column<string>(type: "text", nullable: true),
                    DssModelVersion = table.Column<string>(type: "text", nullable: true),
                    DssName = table.Column<string>(type: "text", nullable: true),
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    PestEppoCode = table.Column<string>(type: "text", nullable: true),
                    ResultMessage = table.Column<string>(type: "text", nullable: true),
                    ResultMessageType = table.Column<int>(type: "integer", nullable: true),
                    WarningMessage = table.Column<string>(type: "text", nullable: true),
                    WarningStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DssResult", x => x.Id);
                });
        }
    }
}
