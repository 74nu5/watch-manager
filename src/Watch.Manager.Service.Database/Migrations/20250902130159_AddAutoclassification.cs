using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.Manager.Service.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoclassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AutoThreshold",
                table: "Categories",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Embedding",
                table: "Categories",
                type: "vector(1536)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ManualThreshold",
                table: "Categories",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoThreshold",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ManualThreshold",
                table: "Categories");
        }
    }
}
