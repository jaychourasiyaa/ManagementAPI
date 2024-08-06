using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class updatedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TasksReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TasksReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TasksReviews_ReviewBy",
                table: "TasksReviews",
                column: "ReviewBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksReviews_Employees_ReviewBy",
                table: "TasksReviews",
                column: "ReviewBy",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksReviews_Employees_ReviewBy",
                table: "TasksReviews");

            migrationBuilder.DropIndex(
                name: "IX_TasksReviews_ReviewBy",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TasksReviews");
        }
    }
}
