using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class GetAttendenceDto
    {
        public DateOnly date { get; set; }
        public AttendenceStatus status { get; set; }

    }
}
