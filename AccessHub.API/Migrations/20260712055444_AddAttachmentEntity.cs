using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Attachments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_RequestId",
                table: "Attachments",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AccessRequests_RequestId",
                table: "Attachments",
                column: "RequestId",
                principalTable: "AccessRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AccessRequests_RequestId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_RequestId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Attachments");
        }
    }
}
