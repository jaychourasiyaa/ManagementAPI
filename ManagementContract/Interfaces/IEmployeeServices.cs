using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIEmployee;
using Employee_Role;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Dtos;
namespace ManagementAPI.Contract.Interfaces
{
    public interface IEmployeeServices
    {
        public  Task<(int,List<GetEmployeeDto>)> GetAllEmployee(PaginatedGetDto PDto);
        public Task<GetByIdDto> GetDetailsById(int id);
        public Task<List<GetEmployeeDto>> GetAllManagers();
        public Task<int?> GetCountRoleWise(int role);
        public Task<int> AddEmployee(AddEmployeeDto addemployeeDto);
        public Task<int> UpdateEmployee(AddEmployeeDto addemployeeDto, int id);
        public Task<bool> DeleteEmployee(int id);

        /*public Task<List<IdandNameDto>> GetDeletedEmployee();
        public  Task<bool> ReactivateEmployee(int id);*/


    }
}
