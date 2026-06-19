using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BapMate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameGroupLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "GameHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupTitle",
                table: "GameHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillSplitGameId",
                table: "Groups",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GameHistories");

            migrationBuilder.DropColumn(
                name: "GroupTitle",
                table: "GameHistories");

            migrationBuilder.DropColumn(
                name: "BillSplitGameId",
                table: "Groups");
        }
    }
}
