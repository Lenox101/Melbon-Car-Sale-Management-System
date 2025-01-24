using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melbon_Car_Sale_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EngineSize",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineSize",
                table: "Cars");
        }
    }
}
