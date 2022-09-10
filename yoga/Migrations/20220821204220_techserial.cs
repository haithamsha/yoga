using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class techserial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "TechearMemberShips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "TechearMemberShips");
        }
    }
}
