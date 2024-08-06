using ManagementAPI.Contract.Dtos;
using ManagementAPIDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Dtos;


namespace ManagementAPI.Contract.Interfaces
{
    public interface IDepartmentServices
    {
        public Task<List<DepartmentDtos>?> GetDepartment();
        public Task<int> AddDepartment(AddDepartmentDtos departmentDtos , int createdBy);
        public Task<bool> DeleteDepartment(int id , int deletedBy);
        public Task<List<EmployeeDto>> GetEmployeeDetails(int id);



    }
}
