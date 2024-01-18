using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class FieldFieldCrop1To1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FieldCrop_FieldId",
                table: "FieldCrop");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCrop_FieldId",
                table: "FieldCrop",
                column: "FieldId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FieldCrop_FieldId",
                table: "FieldCrop");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCrop_FieldId",
                table: "FieldCrop",
                column: "FieldId");
        }
    }
}
