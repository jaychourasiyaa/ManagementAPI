using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Employee_Role;
using ManagementAPI.Contract.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
namespace ManagementAPI.Contract.Dtos
{
    public class EmployeeDto : BaseEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public string? DepartmentName { get; set; }
        public string? AdminName { get; set; }
        public int? DepartmentId { get; set; }
        public int? AdminId { get; set; }
        
        public bool IsActive { get; set; }
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;

    }
}
