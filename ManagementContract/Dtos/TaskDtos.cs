using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;
using TasksReviewAPI;

namespace ManagementAPI.Contract.Dtos
{
    public  class TaskDtos
    {
        public int ? Id { get; set; }
        public string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public required int AssignedToId { get; set; }
        public required int AssignedById { get; set; }
        public string? Assigned_From { get; set; }
        public string? Assigned_To { get; set; }
        public TasksStatus Status { get; set; } = TasksStatus.Pending;
        public bool isActive { get; set; } =true;
    }
}
