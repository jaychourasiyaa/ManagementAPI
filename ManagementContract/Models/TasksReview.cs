using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksAPI;
using ManagementAPIEmployee;
using ManagementAPI.Contract.Models;
namespace TasksReviewAPI

{
    public class TasksReview  : BaseEntity
    {
        
        public int Id { get; set; }
        public int TasksId { get; set; } 
        public int ReviewBy {  get; set; }

        public string Comments { get; set; }

        [ForeignKey(nameof(TasksId))]
        public Tasks Tasks { get; set; }
        [ForeignKey(nameof(ReviewBy))]
        public Employee Reviewer { get; set; }

    }
}
