using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class adde1twoMRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Taasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taasks_ProjectId",
                table: "Taasks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taasks_Projects_ProjectId",
                table: "Taasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taasks_Projects_ProjectId",
                table: "Taasks");

            migrationBuilder.DropIndex(
                name: "IX_Taasks_ProjectId",
                table: "Taasks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Taasks");

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    TasksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => new { x.ProjectId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Taasks_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Taasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }
    }
}
