using Employee_Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class LoginResponseDto
    {
        public string Name { get; set; }
        public int  EmployeeId    { get; set; }
        public EmployeeRole Role {  get; set; }
       
        public string Token { get; set; }
    }
}
