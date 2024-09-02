using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.TaskReviewDtos
{
    public class UpdateTaskReview
    {
        [Required(ErrorMessage = "TaskId is a required field")]
        [Range(1, int.MaxValue, ErrorMessage = "Task Id should be greater than 0")]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "CommentId is a required field")]
        [Range(1, int.MaxValue, ErrorMessage = "CommentId should be greater than 0")]
        public int CommentId { get; set; }
        public string? Comment { get; set; }

    }
}
