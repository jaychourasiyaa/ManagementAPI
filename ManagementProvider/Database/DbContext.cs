using ManagementAPI.Contract.Models;
using ManagementAPIDepartment;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore;
using TasksAPI;
using TasksReviewAPI;
namespace ManagementAPI.Provider.Database
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tasks> Taasks { get; set; }
        public DbSet<TasksReview> TasksReviews { get; set; }
        public DbSet<Attendence> Attendences { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            modelBuilder.Entity<Tasks>()
           .HasOne(t => t.AssignedTo)
           .WithMany()
           .HasForeignKey(t => t.AssignedToId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.AssignedBy)
                .WithMany()
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Attendence>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId).
                OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Salary>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
           
            modelBuilder.Entity<Project>()
                .HasOne(e => e.ProjectMaker)
                .WithMany()
                .HasForeignKey(e => e.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);
           
            modelBuilder.Entity<ProjectEmployee>()
                .HasKey(e => new { e.EmployeeID, e.ProjectID });

            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(e => e.Project)
                .WithMany( e=> e.ProjectEmployee)
                .HasForeignKey( e => e.ProjectID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(e => e.Employee)
                .WithMany(e => e.ProjectEmployees)
                .HasForeignKey(e => e.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Project>()
           .HasMany(p => p.Taskss)
           .WithOne(t => t.Project)
           .HasForeignKey(t => t.ProjectId);
            modelBuilder.Entity<Tasks>()
                .HasMany( t => t.Reviews)
                .WithOne( t => t.Tasks)
                .HasForeignKey( t => t.TasksId)
                .OnDelete( DeleteBehavior.Restrict);
            

        }
    }
    

}