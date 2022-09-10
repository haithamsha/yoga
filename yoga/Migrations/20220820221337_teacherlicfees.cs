using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class teacherlicfees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExamFeesPrice",
                table: "TechearMemberShips",
                newName: "LicenseFeesPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseFeesPrice",
                table: "TechearMemberShips",
                newName: "ExamFeesPrice");
        }
    }
}
