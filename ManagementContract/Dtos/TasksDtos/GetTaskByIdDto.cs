using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;
using TasksReviewAPI;

namespace ManagementAPI.Contract.Dtos.TasksDtos
{
    public class GetTaskByIdDto
    {
        
        public string Name { get; set; }
        public string ? Assigned_From { get; set; }
        public string ? Assigned_To { get; set; }
        public DateTime CreatedOn { get; set; } 
        public string Description { get; set; }
        public TasksStatus Status { get; set; }
        public TaskTypes Type { get; set; }
        public int EstimatedHours { get; set; }
        public int RemainingHours { get; set; }
        public  ICollection<TasksReview> Reviews { get; set; }
    }
}
