﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melbon_Car_Sale_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCarImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Cars",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Cars");
        }
    }
}
