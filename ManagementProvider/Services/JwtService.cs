using Employee_Role;
using ManagementAPI.Contract.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ManagementAPI.Provider.Services
{
    public class JwtService : IJwtService
    {
        public int UserId {  get; set; }
        public EmployeeRole UserRole { get; set; }
        public JwtService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if( user != null)
            {
                var idClaim = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                UserId = idClaim != null ? int.Parse(idClaim): 0;
                var RoleClaim= user.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                if(Enum.TryParse(typeof(EmployeeRole) ,RoleClaim , true, out var RoleEnum))
                {
                  UserRole = (EmployeeRole)RoleEnum;
                }

            }
        }
    }
}
