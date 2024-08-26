using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class PaginatedGetDto
    {
        public string? filterOn { get; set; } = null;
        public string? filterQuery { get; set; } = null; 
        public  string ? SortBy { get; set; } = null;
        public bool   IsAscending { get; set; } = true;
        public int  pageNumber { get; set; } = 1;
        public int  pageSize { get; set; } = 10;
        public string ? additionalSearch { get; set; } = null;
        public DateTime? startDate { get; set; } = null;
        public DateTime? endDate { get; set; }
    }
}
