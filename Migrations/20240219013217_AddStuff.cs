using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardChecklistItem_CardChecklists_CardChecklistId",
                table: "CardChecklistItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CardChecklistItem_Users_UserId",
                table: "CardChecklistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardChecklistItem",
                table: "CardChecklistItem");

            migrationBuilder.RenameTable(
                name: "CardChecklistItem",
                newName: "CardChecklistItems");

            migrationBuilder.RenameIndex(
                name: "IX_CardChecklistItem_UserId",
                table: "CardChecklistItems",
                newName: "IX_CardChecklistItems_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CardChecklistItem_CardChecklistId",
                table: "CardChecklistItems",
                newName: "IX_CardChecklistItems_CardChecklistId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CardChecklistItems",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardChecklistItems",
                table: "CardChecklistItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardChecklistItems_CardChecklists_CardChecklistId",
                table: "CardChecklistItems",
                column: "CardChecklistId",
                principalTable: "CardChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CardChecklistItems_Users_UserId",
                table: "CardChecklistItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardChecklistItems_CardChecklists_CardChecklistId",
                table: "CardChecklistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CardChecklistItems_Users_UserId",
                table: "CardChecklistItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardChecklistItems",
                table: "CardChecklistItems");

            migrationBuilder.RenameTable(
                name: "CardChecklistItems",
                newName: "CardChecklistItem");

            migrationBuilder.RenameIndex(
                name: "IX_CardChecklistItems_UserId",
                table: "CardChecklistItem",
                newName: "IX_CardChecklistItem_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CardChecklistItems_CardChecklistId",
                table: "CardChecklistItem",
                newName: "IX_CardChecklistItem_CardChecklistId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CardChecklistItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardChecklistItem",
                table: "CardChecklistItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardChecklistItem_CardChecklists_CardChecklistId",
                table: "CardChecklistItem",
                column: "CardChecklistId",
                principalTable: "CardChecklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CardChecklistItem_Users_UserId",
                table: "CardChecklistItem",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
