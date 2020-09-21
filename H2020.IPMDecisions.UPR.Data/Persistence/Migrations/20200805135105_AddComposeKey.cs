using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddComposeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest");

            migrationBuilder.DropIndex(
                name: "IX_DataSharingRequest_RequesteeId",
                table: "DataSharingRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest",
                columns: new[] { "RequesteeId", "RequesterId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequest_RequesteeId",
                table: "DataSharingRequest",
                column: "RequesteeId");
        }
    }
}
