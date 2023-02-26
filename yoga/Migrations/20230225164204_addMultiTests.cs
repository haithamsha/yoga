using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yoga.Migrations
{
    /// <inheritdoc />
    public partial class addMultiTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "techearMemberShipTests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechearMemberShipMemId = table.Column<int>(type: "int", nullable: false),
                    MemId = table.Column<int>(type: "int", nullable: false),
                    TeachingType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceiptCopy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptCopyLic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayFees = table.Column<bool>(type: "bit", nullable: false),
                    TakeExam = table.Column<bool>(type: "bit", nullable: false),
                    PassExam = table.Column<bool>(type: "bit", nullable: false),
                    FinalApprove = table.Column<bool>(type: "bit", nullable: false),
                    PayExamFees = table.Column<bool>(type: "bit", nullable: false),
                    LicenseFeesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExamLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_techearMemberShipTests", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_techearMemberShipTests_TechearMemberShips_TechearMemberShipMemId",
                        column: x => x.TechearMemberShipMemId,
                        principalTable: "TechearMemberShips",
                        principalColumn: "MemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_techearMemberShipTests_TechearMemberShipMemId",
                table: "techearMemberShipTests",
                column: "TechearMemberShipMemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "techearMemberShipTests");
        }
    }
}
