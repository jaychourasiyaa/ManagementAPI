﻿// <auto-generated />
using System;
using ManagementAPI.Provider.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ManagementAPI.Provider.Migrations
{
    [DbContext(typeof(dbContext))]
    [Migration("20240727095849_SalarySectionAdded")]
    partial class SalarySectionAdded
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ManagementAPIDepartment.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("ManagementAPIEmployee.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<decimal>("Salary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Salary_Limit")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("TasksAPI.Tasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssignedById")
                        .HasColumnType("int");

                    b.Property<int>("AssignedToId")
                        .HasColumnType("int");

                    b.Property<string>("Assigned_From")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Assigned_To")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssignedById");

                    b.HasIndex("AssignedToId");

                    b.ToTable("Taasks");
                });

            modelBuilder.Entity("TasksReviewAPI.TasksReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("On")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReviewBy")
                        .HasColumnType("int");

                    b.Property<int>("TasksId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReviewBy");

                    b.HasIndex("TasksId");

                    b.ToTable("TasksReviews");
                });

            modelBuilder.Entity("ManagementAPIEmployee.Employee", b =>
                {
                    b.HasOne("ManagementAPIEmployee.Employee", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId");

                    b.HasOne("ManagementAPIDepartment.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Admin");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("TasksAPI.Tasks", b =>
                {
                    b.HasOne("ManagementAPIEmployee.Employee", "AssignedBy")
                        .WithMany()
                        .HasForeignKey("AssignedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ManagementAPIEmployee.Employee", "AssignedTo")
                        .WithMany()
                        .HasForeignKey("AssignedToId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssignedBy");

                    b.Navigation("AssignedTo");
                });

            modelBuilder.Entity("TasksReviewAPI.TasksReview", b =>
                {
                    b.HasOne("ManagementAPIEmployee.Employee", "Reviewer")
                        .WithMany()
                        .HasForeignKey("ReviewBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TasksAPI.Tasks", "Tasks")
                        .WithMany()
                        .HasForeignKey("TasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reviewer");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
