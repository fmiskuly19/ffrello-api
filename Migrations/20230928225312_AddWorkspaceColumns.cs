using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWatched",
                table: "Cards");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Workspaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "Workspaces",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsStarred",
                value: true);

            migrationBuilder.UpdateData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Theme" },
                values: new object[] { null, "Fwank", null });

            migrationBuilder.InsertData(
                table: "Workspaces",
                columns: new[] { "Id", "Description", "Name", "Theme" },
                values: new object[,]
                {
                    { 2, null, "Cafwin", null },
                    { 3, null, "M.C.", null }
                });

            migrationBuilder.InsertData(
                table: "Boards",
                columns: new[] { "Id", "IsStarred", "Name", "WorkspaceId" },
                values: new object[,]
                {
                    { 4, true, "Peaches Board", 2 },
                    { 5, false, "Painting Board", 2 },
                    { 6, false, "Prill Board", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_Name",
                table: "Workspaces",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workspaces_Name",
                table: "Workspaces");

            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "Workspaces");

            migrationBuilder.AddColumn<bool>(
                name: "IsWatched",
                table: "Cards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsStarred",
                value: false);

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsWatched",
                value: false);

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsWatched",
                value: false);

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsWatched",
                value: false);

            migrationBuilder.UpdateData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Franks Workspace");
        }
    }
}
