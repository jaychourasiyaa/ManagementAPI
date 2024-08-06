using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;

namespace ManagementAPI.Contract.Dtos
{
    public class AddTasksDtos
    {
        [Required (ErrorMessage = "Task Name is a required field")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 15 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is a required field")]
        public string Description { get; set; }
        [Required(ErrorMessage = "AssignedToId By is required field")]
        public int AssignedToId { get; set; }
        [Required(ErrorMessage = "AssignedById is required field")]
        
        [Range ( 0 , 2 ,ErrorMessage = "Role is invalid should be between 0-2")]
       public TasksStatus Status { get; set; } = TasksStatus.Pending;

    }
}
