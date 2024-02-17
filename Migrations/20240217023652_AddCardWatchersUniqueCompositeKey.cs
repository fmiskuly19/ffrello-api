using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCardWatchersUniqueCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CardWatchers",
                table: "CardWatchers");

            migrationBuilder.DropIndex(
                name: "IX_CardWatchers_UserId",
                table: "CardWatchers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CardWatchers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardWatchers",
                table: "CardWatchers",
                columns: new[] { "UserId", "CardId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CardWatchers",
                table: "CardWatchers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CardWatchers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardWatchers",
                table: "CardWatchers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CardWatchers_UserId",
                table: "CardWatchers",
                column: "UserId");
        }
    }
}
