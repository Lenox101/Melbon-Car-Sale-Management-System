using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melbon_Car_Sale_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCarCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Cars");
        }
    }
}
