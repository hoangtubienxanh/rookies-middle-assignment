using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScribeDbManager.Migrations
{
    /// <inheritdoc />
    public partial class Public_DbSet_ApplicationItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationItem_Books_BookId",
                table: "ApplicationItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationItem_LoanApplications_LoanApplicationId",
                table: "ApplicationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationItem",
                table: "ApplicationItem");

            migrationBuilder.RenameTable(
                name: "ApplicationItem",
                newName: "ApplicationItems");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationItem_LoanApplicationId",
                table: "ApplicationItems",
                newName: "IX_ApplicationItems_LoanApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationItem_BookId",
                table: "ApplicationItems",
                newName: "IX_ApplicationItems_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationItems",
                table: "ApplicationItems",
                column: "ApplicationItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationItems_Books_BookId",
                table: "ApplicationItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationItems_LoanApplications_LoanApplicationId",
                table: "ApplicationItems",
                column: "LoanApplicationId",
                principalTable: "LoanApplications",
                principalColumn: "LoanApplicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationItems_Books_BookId",
                table: "ApplicationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationItems_LoanApplications_LoanApplicationId",
                table: "ApplicationItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationItems",
                table: "ApplicationItems");

            migrationBuilder.RenameTable(
                name: "ApplicationItems",
                newName: "ApplicationItem");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationItems_LoanApplicationId",
                table: "ApplicationItem",
                newName: "IX_ApplicationItem_LoanApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationItems_BookId",
                table: "ApplicationItem",
                newName: "IX_ApplicationItem_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationItem",
                table: "ApplicationItem",
                column: "ApplicationItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationItem_Books_BookId",
                table: "ApplicationItem",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationItem_LoanApplications_LoanApplicationId",
                table: "ApplicationItem",
                column: "LoanApplicationId",
                principalTable: "LoanApplications",
                principalColumn: "LoanApplicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
