using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailToVmTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccreditedHours",
                table: "techearMemberShipTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertaficateDate",
                table: "techearMemberShipTests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CertficateFiles",
                table: "techearMemberShipTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolLink",
                table: "techearMemberShipTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolLocation",
                table: "techearMemberShipTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolName",
                table: "techearMemberShipTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolSocialMediaAccount",
                table: "techearMemberShipTests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccreditedHours",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "CertaficateDate",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "CertficateFiles",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "SchoolLink",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "SchoolLocation",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "SchoolName",
                table: "techearMemberShipTests");

            migrationBuilder.DropColumn(
                name: "SchoolSocialMediaAccount",
                table: "techearMemberShipTests");
        }
    }
}
