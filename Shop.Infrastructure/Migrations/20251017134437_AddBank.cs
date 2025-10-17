using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "Shops",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "Shops");
        }
    }
}
