using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class Widget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Widget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.Id);
                    table.UniqueConstraint("AK_Widget_Description", x => x.Description);
                });

            migrationBuilder.CreateTable(
                name: "UserWidget",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    WidgetId = table.Column<int>(nullable: false),
                    WidgetDescription = table.Column<string>(nullable: false),
                    Allowed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWidget", x => new { x.UserId, x.WidgetId });
                    table.ForeignKey(
                        name: "FK_UserWidget_User",
                        column: x => x.UserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWidget_Widget",
                        column: x => x.WidgetDescription,
                        principalTable: "Widget",
                        principalColumn: "Description",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Widget",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 0, "Maps" },
                    { 1, "Actions" },
                    { 2, "RiskForecast" },
                    { 3, "Weather" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWidget_WidgetDescription",
                table: "UserWidget",
                column: "WidgetDescription");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_Description",
                table: "Widget",
                column: "Description",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWidget");

            migrationBuilder.DropTable(
                name: "Widget");
        }
    }
}
