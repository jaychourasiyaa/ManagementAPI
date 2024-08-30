using ManagementAPI.Contract.Enums;
using ManagementAPI.Contract.Models;
using Taask_status;

namespace ManagementAPI.Contract.Dtos
{
    public class GetTaskDto
    {
        public int Id { get; set; }
        public string Name { get ; set; }
        public string Assigned_From { get; set; }
        public string Assigned_To  { get; set; }
        public int AssignedById  { get; set; }
        public int? AssignedToId   { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Description  { get; set; }
        public TasksStatus Status  { get; set; }
        public bool IsActive { get; set; }
        public int  ProjectId { get; set; }
        public int ? ParentId  { get; set; }
        public int ? SprintId { get; set; }
        public TaskTypes Type { get; set; }
        public int EstimatedHours { get; set; }
        public int RemainingHours { get; set; }
    }
}
