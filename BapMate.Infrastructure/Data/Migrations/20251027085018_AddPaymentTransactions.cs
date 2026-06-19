using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BapMate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Direction = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    WalletDelta = table.Column<decimal>(type: "TEXT", nullable: false),
                    EscrowDelta = table.Column<decimal>(type: "TEXT", nullable: false),
                    WalletBalanceAfter = table.Column<decimal>(type: "TEXT", nullable: false),
                    EscrowBalanceAfter = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Counterparty = table.Column<string>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<string>(type: "TEXT", nullable: true),
                    ReferenceType = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");
        }
    }
}
