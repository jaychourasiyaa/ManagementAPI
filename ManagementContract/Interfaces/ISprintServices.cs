using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ISprintServices
    {
        public Task<int> AddSprint( AddSprintDto dto ,int ? toBeUpdated);
        public Task<List<GetSprintDto>?> GetSprintUnderProject(int ?id);
    }
}
