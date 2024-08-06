using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIEmployee;
using Employee_Role;
namespace ManagementAPI.Contract.Interfaces
{
    public interface IEmployeeServices
    {
        public  Task<int> AddEmployee(AddEmployeeDtos addemployeeDto, int createdBy);
        public Task<int> UpdateEmployee(AddEmployeeDtos addemployeeDto, int id , int updatedBy);
        public Task<bool> DeleteEmployee(int id, int deletedBy);
        public  Task<List<EmployeeDto>> GetAllEmployee(EmployeeRole Role,int managerId);

        public Task<EmployeeDto > GetDetailsById(int id);
        public Task<List<EmployeeDto>> GetAllManagers();


    }
}
