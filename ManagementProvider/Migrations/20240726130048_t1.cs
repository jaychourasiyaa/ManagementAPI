using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class t1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Taasks_AssignedById",
                table: "Taasks",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Taasks_AssignedToId",
                table: "Taasks",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taasks_Employees_AssignedById",
                table: "Taasks",
                column: "AssignedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Taasks_Employees_AssignedToId",
                table: "Taasks",
                column: "AssignedToId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taasks_Employees_AssignedById",
                table: "Taasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Taasks_Employees_AssignedToId",
                table: "Taasks");

            migrationBuilder.DropIndex(
                name: "IX_Taasks_AssignedById",
                table: "Taasks");

            migrationBuilder.DropIndex(
                name: "IX_Taasks_AssignedToId",
                table: "Taasks");
        }
    }
}
