using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class UpdateddateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date_Time",
                table: "Attendences");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Attendences",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Attendences");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Time",
                table: "Attendences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
