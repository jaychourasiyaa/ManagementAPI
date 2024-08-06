using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ISalaryServices
    {
        public Task< List<DateTime>> getById(int id);
        public Task<int> TakeAdvance(SalaryDtos dtos);
    }
}
