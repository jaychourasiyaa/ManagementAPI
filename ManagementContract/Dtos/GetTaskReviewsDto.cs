using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;
namespace ManagementAPI.Contract.Dtos
{
    public class GetTaskReviewDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Assigned_From { get; set; }
        public string Assigned_To{ get; set; }
        public int AssignedById { get; set; }
        public int AssignedToId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public TasksStatus Status { get; set; }
        public int ReviewById {  get ; set; }
        public string ReviewerName { get; set; }
        public string Comments { get; set; }
    }
}
