using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;
namespace ManagementAPI.Contract.Dtos.TaskReviewDtos
{
    public class GetTaskReviewDto
    {
        public int Id { get; set; }
        //public int TaskId { get; set; }
        // int ReviewById {  get ; set; }
        public string ReviewerName { get; set; }
        public string Comments { get; set; }
        public DateTime dateTime { get; set; }
        //public int AssignedById { get; set; }
        //public int ? AssignedToId { get; set; }
    }
}
