using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCardChecklsits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CardComments");

            migrationBuilder.DropColumn(
                name: "UserProfilePhotoUrl",
                table: "CardComments");

            migrationBuilder.CreateTable(
                name: "CardChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardChecklistItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsChecked = table.Column<bool>(type: "INTEGER", nullable: false),
                    CardChecklistId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardChecklistItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardChecklistItem_CardChecklists_CardChecklistId",
                        column: x => x.CardChecklistId,
                        principalTable: "CardChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardChecklistItem_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "ProfilePhotoUrl" },
                values: new object[] { "Frank M", "https://lh3.googleusercontent.com/a/ACg8ocIJ36231TQrGFILAqYBP5CXuKJhnxqtHt4MJuT7GtUgOg=s96-c" });

            migrationBuilder.CreateIndex(
                name: "IX_CardChecklistItem_CardChecklistId",
                table: "CardChecklistItem",
                column: "CardChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_CardChecklistItem_UserId",
                table: "CardChecklistItem",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardChecklistItem");

            migrationBuilder.DropTable(
                name: "CardChecklists");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CardComments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserProfilePhotoUrl",
                table: "CardComments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "ProfilePhotoUrl" },
                values: new object[] { "", "" });
        }
    }
}
