using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddDisabledDssTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisabledDss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DssId = table.Column<string>(nullable: false),
                    DssVersion = table.Column<string>(nullable: false),
                    DssModelId = table.Column<string>(nullable: false),
                    DssModelVersion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledDss", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisabledDss");
        }
    }
}
