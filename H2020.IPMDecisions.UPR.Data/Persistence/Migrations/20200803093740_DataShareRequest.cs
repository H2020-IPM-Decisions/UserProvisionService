using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class DataShareRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataSharingRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSharingRequestStatus", x => x.Id);
                    table.UniqueConstraint("AK_DataSharingRequestStatus_Description", x => x.Description);
                });

            migrationBuilder.CreateTable(
                name: "DataSharingRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RequesteeId = table.Column<Guid>(nullable: false),
                    RequesterId = table.Column<Guid>(nullable: false),
                    RequestStatusDescription = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSharingRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSharingRequest_RequestStatus_RequestDescription",
                        column: x => x.RequestStatusDescription,
                        principalTable: "DataSharingRequestStatus",
                        principalColumn: "Description");
                    table.ForeignKey(
                        name: "FK_DataSharingRequest_UserProfile_RequesteeId",
                        column: x => x.RequesteeId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataSharingRequest_UserProfile_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DataSharingRequestStatus",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 0, "Pending" },
                    { 1, "Accepted" },
                    { 2, "Declined" },
                    { 3, "Revoked" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequest_RequestStatusDescription",
                table: "DataSharingRequest",
                column: "RequestStatusDescription");

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequest_RequesteeId",
                table: "DataSharingRequest",
                column: "RequesteeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequest_RequesterId",
                table: "DataSharingRequest",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSharingRequestStatus_Description",
                table: "DataSharingRequestStatus",
                column: "Description",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSharingRequest");

            migrationBuilder.DropTable(
                name: "DataSharingRequestStatus");
        }
    }
}
