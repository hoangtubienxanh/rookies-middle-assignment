using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScribeDbManager.Migrations
{
    /// <inheritdoc />
    public partial class Navigation_Loan_LoanApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanApplications_LoanApplicationId",
                table: "Loans");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanApplications_LoanApplicationId",
                table: "Loans",
                column: "LoanApplicationId",
                principalTable: "LoanApplications",
                principalColumn: "LoanApplicationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanApplications_LoanApplicationId",
                table: "Loans");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanApplications_LoanApplicationId",
                table: "Loans",
                column: "LoanApplicationId",
                principalTable: "LoanApplications",
                principalColumn: "LoanApplicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
