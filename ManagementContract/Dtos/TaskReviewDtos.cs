using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class TaskReviewDtos
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
         public string Name { get; set; }
        public string Task_Description { get; set; }
        public int ReviewBy { get; set; }

       
        public string ? Reviewer_Name { get; set; }    
        public DateTime On { get; set; } = DateTime.Now;
        public string? Comments { get; set; }
    }
}
