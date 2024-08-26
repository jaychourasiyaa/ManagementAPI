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
        public  Task<int> AddEmployee(AddEmployeeDtos addemployeeDto, int createdBy);
        public Task<int> UpdateEmployee(AddEmployeeDtos addemployeeDto, int id , int updatedBy);
        public Task<bool> DeleteEmployee(int id, int deletedBy);
        public  Task<(int,List<GetEmployeeDto>)> GetAllEmployee(EmployeeRole Role,int managerId , PaginatedGetDto PDto);

        public Task<GetByIdDto> GetDetailsById(int id);
        public Task<List<GetEmployeeDto>> GetAllManagers();
        public Task<List<IdandNameDto>> GetDeletedEmployee();
        public  Task<bool> ReactivateEmployee(int id);
        public Task<int?> GetCountRoleWise(int role);

    }
}
