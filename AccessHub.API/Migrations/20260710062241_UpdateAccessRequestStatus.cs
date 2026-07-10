using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessHub.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccessRequestStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "RequestApprovals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AccessRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovals_UserId",
                table: "RequestApprovals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_UserId",
                table: "AccessRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_Users_UserId",
                table: "AccessRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApprovals_Users_UserId",
                table: "RequestApprovals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_Users_UserId",
                table: "AccessRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApprovals_Users_UserId",
                table: "RequestApprovals");

            migrationBuilder.DropIndex(
                name: "IX_RequestApprovals_UserId",
                table: "RequestApprovals");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_UserId",
                table: "AccessRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RequestApprovals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AccessRequests");
        }
    }
}
