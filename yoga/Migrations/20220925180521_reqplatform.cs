using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class reqplatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Platforms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements_Ar",
                table: "Platforms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Requirements_Ar",
                table: "Platforms");
        }
    }
}
