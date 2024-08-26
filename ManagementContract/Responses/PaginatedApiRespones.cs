using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Responses
{
    
        public class PaginatedApiRespones<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }

            public T? Data { get; set; }
            public int? TotalEntriesCount { get; set; }
            public PaginatedApiRespones() { Success = true; Message = null; }

            public PaginatedApiRespones(bool success, string message, T data, int count)
            {
                Success = success;
                Message = message;
                TotalEntriesCount = count;
                Data = data;
            }

        }
    
}
