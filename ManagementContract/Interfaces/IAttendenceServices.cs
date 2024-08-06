using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface IAttendenceServices
    {
       

        public Task<int> AttendenceAdd(AddAttendenceDtos adtos);
        public Task<List<GetAttendenceDto>> getAttendenceList(int id);

    }
}
