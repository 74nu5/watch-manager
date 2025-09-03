using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.Manager.Service.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryHierarchyEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HierarchyLevel",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HierarchyPath",
                table: "Categories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "InheritFromParent",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HierarchyLevel",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HierarchyPath",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "InheritFromParent",
                table: "Categories");
        }
    }
}
