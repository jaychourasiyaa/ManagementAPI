using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.ProjectDtos;
using ManagementAPI.Contract.Models;
using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface IProjectServices
    {
        public Task<int> AddProject(AddProjectDto addProjectDto);
        public Task<ProjectDetailsByIdDto?> GetById(int id);
        public Task<(int,List<GetProjectDetailsDto>?)> GetAllProject(PaginatedGetDto dto);
        public  Task<int?> UpdateProject(int projectId, AddProjectDto dto);
        public Task<bool> DeleteMember(int employeeId,int projectId);
        public Task<int?> GetCountStatusWise(int status);



    }
}
