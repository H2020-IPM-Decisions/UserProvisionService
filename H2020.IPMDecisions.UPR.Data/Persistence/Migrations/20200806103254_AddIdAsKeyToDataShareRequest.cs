using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddIdAsKeyToDataShareRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest");

            migrationBuilder.DeleteData(
                table: "DataSharingRequestStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequest_RequesteeId_RequesterId",
                table: "DataSharingRequest",
                columns: new[] { "RequesteeId", "RequesterId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest");

            migrationBuilder.DropIndex(
                name: "IX_DataSharingRequest_RequesteeId_RequesterId",
                table: "DataSharingRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataSharingRequest",
                table: "DataSharingRequest",
                columns: new[] { "RequesteeId", "RequesterId" });

            migrationBuilder.InsertData(
                table: "DataSharingRequestStatus",
                columns: new[] { "Id", "Description" },
                values: new object[] { 3, "Revoked" });
        }
    }
}
