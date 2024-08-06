using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class addedUpdateNulll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Taasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Taasks");
        }
    }
}
