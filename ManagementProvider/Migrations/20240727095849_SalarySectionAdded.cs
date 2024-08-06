using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class SalarySectionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Salary_Limit",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salary_Limit",
                table: "Employees");
        }
    }
}
