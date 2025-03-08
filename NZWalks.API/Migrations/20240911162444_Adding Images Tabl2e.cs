using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NZWalks.API.Migrations
{
    /// <inheritdoc />
    public partial class AddingImagesTabl2e : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileDescriptipn",
                table: "Images",
                newName: "FileDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileDescription",
                table: "Images",
                newName: "FileDescriptipn");
        }
    }
}
