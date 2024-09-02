using ManagementAPI.Contract.Dtos.LogDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ILoggerServices
    {
        public Task <List<GetLogDto>?>GetLogById(int id);
    }
}
