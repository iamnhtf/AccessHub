using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AccessRequests_RequestId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_RequestId",
                table: "Attachments");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AccessRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "AccessRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "AccessRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AccessRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
