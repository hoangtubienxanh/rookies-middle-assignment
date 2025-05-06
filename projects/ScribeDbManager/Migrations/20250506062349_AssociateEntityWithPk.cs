using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScribeDbManager.Migrations
{
    /// <inheritdoc />
    public partial class AssociateEntityWithPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationItemId",
                table: "ApplicationItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationItem",
                table: "ApplicationItem",
                column: "ApplicationItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationItem",
                table: "ApplicationItem");

            migrationBuilder.DropColumn(
                name: "ApplicationItemId",
                table: "ApplicationItem");
        }
    }
}
