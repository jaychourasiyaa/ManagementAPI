using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class isActiveaddedindepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Department",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Department");
        }
    }
}
