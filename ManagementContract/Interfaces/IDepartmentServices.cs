using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.DepartmentDtos;
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
        public Task<(int,List<GetDepartmentDtos>?)> GetDepartment(PaginatedGetDto dto);
        public Task<List<IdandNameDto>?> GetEmployeeDetails(int id);
        public Task<IdandNameDto> GetDepartmentById(int id);
        public Task<int> AddDepartment(AddDepartmentDto departmentDtos , int createdBy);
        public Task<int?> UpdateDepartment(IdandNameDto dto, int updatedBy);
        public Task<bool> DeleteDepartment(int id , int deletedBy);

        /*public Task<List<IdandNameDto>?> GetDeletedDepartment();
        public Task<bool> ReactivateDepartment(int id);*/
    }
}
