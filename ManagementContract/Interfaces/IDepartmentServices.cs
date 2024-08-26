using ManagementAPI.Contract.Dtos;
using ManagementAPIDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ManagementAPI.Contract.Interfaces
{
    public interface IDepartmentServices
       
    {
        public Task<List<GetDepartmentDtos>?> GetDepartment(PaginatedGetDto dto);
        public Task<int> AddDepartment(AddDepartmentDtos departmentDtos , int createdBy);
        public Task<bool> DeleteDepartment(int id , int deletedBy);
        public Task<List<IdandNameDto>?> GetEmployeeDetails(int id);
        public  Task<int?> UpdateDepartment(IdandNameDto dto, int updatedBy);
        public Task<List<IdandNameDto>?> GetDeletedDepartment();
        public Task<IdandNameDto> GetDepartmentById(int id);
        public Task<bool> ReactivateDepartment(int id);
    }
}
