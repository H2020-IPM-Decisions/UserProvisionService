using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class UpdateUserFarmTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Authorised",
                table: "UserFarm",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserFarmTypeDescription",
                table: "UserFarm",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserFarmType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFarmType", x => x.Id);
                    table.UniqueConstraint("AK_UserFarmType_Description", x => x.Description);
                });

            migrationBuilder.InsertData(
                table: "UserFarmType",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 0, "Unknown" },
                    { 1, "Owner" },
                    { 2, "Advisor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFarm_UserFarmTypeDescription",
                table: "UserFarm",
                column: "UserFarmTypeDescription");

            migrationBuilder.CreateIndex(
                name: "IX_UserFarmType_Description",
                table: "UserFarmType",
                column: "Description",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_UserFarmType_UserFarmTypeDescription",
                table: "UserFarm",
                column: "UserFarmTypeDescription",
                principalTable: "UserFarmType",
                principalColumn: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_UserFarmType_UserFarmTypeDescription",
                table: "UserFarm");

            migrationBuilder.DropTable(
                name: "UserFarmType");

            migrationBuilder.DropIndex(
                name: "IX_UserFarm_UserFarmTypeDescription",
                table: "UserFarm");

            migrationBuilder.DropColumn(
                name: "Authorised",
                table: "UserFarm");

            migrationBuilder.DropColumn(
                name: "UserFarmTypeDescription",
                table: "UserFarm");
        }
    }
}
