using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCardToChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardId",
                table: "CardChecklists",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CardChecklists_CardId",
                table: "CardChecklists",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardChecklists_Cards_CardId",
                table: "CardChecklists",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardChecklists_Cards_CardId",
                table: "CardChecklists");

            migrationBuilder.DropIndex(
                name: "IX_CardChecklists_CardId",
                table: "CardChecklists");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "CardChecklists");
        }
    }
}
