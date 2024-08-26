using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Employee_Role;
using ManagementAPI.Contract.Models;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
namespace ManagementAPI.Contract.Dtos.EmployeeDtos
{
    public class GetEmployeeDto 
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public string? DepartmentName { get; set; }
        public string? AdminName { get; set; }
        public string CreatedBy { get; set; }
        public EmployeeRole Role { get; set; }
        public int? AdminId { get; set; }
        public int ?DepartmentId { get; set; }
        public DateTime CreatedOn { get; set; }
      



    }
}
