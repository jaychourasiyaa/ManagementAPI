using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface IPaginatedService
    {

        public Task<List<DepartmentDtos>?> GetAllDepartment(PaginatedGetDto dto);

        public Task<List<GetEmployeeDto>> GetAllEmployee(PaginatedGetDto dto);
        /*public  Task<int> GetEmployeeCount();
        public  Task<int> GetDepartmentCount()*/
        public Task<List<GetEmployeeDto>?> GetAllManagers(PaginatedGetDto dto);



    }
}
