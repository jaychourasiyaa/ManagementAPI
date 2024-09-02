using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Employee_Role;
using ManagementAPI.Contract.Models;
using ManagementAPIDepartment;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace ManagementAPIEmployee;

public class Employee : BaseEntity
{
   
    public int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Salary { get; set; }
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public Employee Creator { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    
    public Employee Admin { get; set; }
    [ForeignKey(nameof(AdminId))] 
    public int? AdminId { get; set; } // Manager
    
    public Department Department { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }

}
