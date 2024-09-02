using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.DepartmentDtos
{
    public class PaginatedGetDepartmentDto
    {

        public string? SortBy { get; set; } = null;
        public bool IsAscending { get; set; } = true;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
