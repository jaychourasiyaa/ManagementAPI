using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ISortingService
    {
        public IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, bool isAscending);
    }
}
