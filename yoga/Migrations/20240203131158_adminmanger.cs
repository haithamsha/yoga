using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class adminmanger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUserId",
                table: "Notification",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AdminUserId",
                table: "Notification",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_AdminUserId",
                table: "Notification",
                column: "AdminUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_AdminUserId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AdminUserId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Notification");
        }
    }
}
