using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class addedBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assigned_From",
                table: "Taasks");

            migrationBuilder.DropColumn(
                name: "Assigned_To",
                table: "Taasks");

            migrationBuilder.DropColumn(
                name: "Salary_Limit",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "UpdateOn",
                table: "Taasks",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Employees",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Employees",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Department",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TasksReviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TasksReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "TasksReviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "TasksReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Taasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Taasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Department",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Department",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Department",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "TasksReviews");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Taasks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Taasks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Department");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Taasks",
                newName: "UpdateOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Employees",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Employees",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Department",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Assigned_From",
                table: "Taasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assigned_To",
                table: "Taasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary_Limit",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
