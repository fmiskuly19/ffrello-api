using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardListNameToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoardListName",
                table: "Cards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1,
                column: "BoardListName",
                value: "TODO");

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 2,
                column: "BoardListName",
                value: "In Progress");

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 3,
                column: "BoardListName",
                value: "DONE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardListName",
                table: "Cards");
        }
    }
}
