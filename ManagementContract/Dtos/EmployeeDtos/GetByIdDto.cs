using Employee_Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.EmployeeDtos
{
    public class GetByIdDto
    {
       /* "username": "string",
  "password": "string",
  "name": "srpfoqynoICJHAhCjkPenfWcl",
  "salary": 1,
  "departmentId": 0,
  "adminId": 0,
  "role": 0*/
        public string userName { get; set; }
        public string password { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public EmployeeRole Role { get; set; }
        public string ? AdminName { get; set; }
        public string ? DepartmentName { get; set; }
       public int ?DepartmentId { get; set; }
        public int ?AdminId  { get; set; }
    }
}
