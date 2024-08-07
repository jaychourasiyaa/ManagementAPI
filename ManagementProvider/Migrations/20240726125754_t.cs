﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class t : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_AdminId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_AdminId",
                table: "Employees",
                column: "AdminId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_AdminId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_AdminId",
                table: "Employees",
                column: "AdminId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
