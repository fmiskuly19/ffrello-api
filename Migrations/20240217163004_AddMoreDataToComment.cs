using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFrelloApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDataToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CardComments");

            migrationBuilder.DropColumn(
                name: "UserProfilePhotoUrl",
                table: "CardComments");
        }
    }
}
