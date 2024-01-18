using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    public partial class AddUserIdAsey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesteeId",
                table: "DataSharingRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesterId",
                table: "DataSharingRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_UserProfile_UserProfileId",
                table: "UserFarm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm");

            migrationBuilder.DropIndex(
                name: "IX_UserFarm_UserProfileId",
                table: "UserFarm");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "UserFarm");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserFarm",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm",
                columns: new[] { "UserId", "FarmId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesteeId",
                table: "DataSharingRequest",
                column: "RequesteeId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesterId",
                table: "DataSharingRequest",
                column: "RequesterId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_UserProfile_UserId",
                table: "UserFarm",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesteeId",
                table: "DataSharingRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesterId",
                table: "DataSharingRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFarm_UserProfile_UserId",
                table: "UserFarm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserProfile",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserFarm",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "UserFarm",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFarm",
                table: "UserFarm",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserFarm_UserProfileId",
                table: "UserFarm",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesteeId",
                table: "DataSharingRequest",
                column: "RequesteeId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DataSharingRequest_UserProfile_RequesterId",
                table: "DataSharingRequest",
                column: "RequesterId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFarm_UserProfile_UserProfileId",
                table: "UserFarm",
                column: "UserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
