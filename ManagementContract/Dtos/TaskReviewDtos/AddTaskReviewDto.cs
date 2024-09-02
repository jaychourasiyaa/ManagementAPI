using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksAPI;

namespace ManagementAPI.Contract.Dtos.TaskReviewDtos
{
    public class AddTaskReviewDto
    {
        [Required(ErrorMessage = "Task Id is a required feild")]
        [Range(1, int.MaxValue, ErrorMessage = " Id should be greater than 0")]
        public int TasksId { get; set; }

        [Required(ErrorMessage = "Comments is a required field")]
        public string Comments { get; set; }

    }
}
