using ManagementAPI.Contract.Dtos.SprintDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ISprintServices
    {
        public Task<List<GetSprintDto>?> GetSprintUnderProject(int? id);
        public Task<int> AddSprint( AddSprintDto dto ,int ? toBeUpdated);
    }
}
