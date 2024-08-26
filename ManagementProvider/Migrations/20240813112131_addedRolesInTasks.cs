using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class addedRolesInTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Taasks_ParentId",
                table: "Taasks",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taasks_Taasks_ParentId",
                table: "Taasks",
                column: "ParentId",
                principalTable: "Taasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taasks_Taasks_ParentId",
                table: "Taasks");

            migrationBuilder.DropIndex(
                name: "IX_Taasks_ParentId",
                table: "Taasks");
        }
    }
}
