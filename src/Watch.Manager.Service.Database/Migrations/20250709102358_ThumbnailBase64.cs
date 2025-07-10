using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.Manager.Service.Database.Migrations
{
    /// <inheritdoc />
    public partial class ThumbnailBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailBase64",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailBase64",
                table: "Articles");
        }
    }
}
