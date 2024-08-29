using Employee_Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface IJwtService
    {
       public int UserId { get; }
       public EmployeeRole UserRole { get; }
    }

}
