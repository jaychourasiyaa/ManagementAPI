using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Models;
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
        public Task<GetProjectDetailsDto?> GetById(int id);
        public Task<List<GetProjectDetailsDto>?> GetAllProject();

    }
}
