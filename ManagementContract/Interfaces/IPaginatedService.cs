using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
   public interface IPaginatedService
    {

        public Task<List<DepartmentDtos>?> GetAllDepartment(PaginatedGetDepartmentDto dto);

        public Task<List<EmployeeDto>> GetAllEmployee(PaginatedGetDto dto);
        /*public  Task<int> GetEmployeeCount();
        public  Task<int> GetDepartmentCount()*/
        public Task<List<EmployeeDto>?> GetAllManagers(PaginatedGetDto dto);



    }
}
