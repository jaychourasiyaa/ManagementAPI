using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;

namespace ManagementAPI.Contract.Dtos.TasksDtos
{
    public class AddTasksDto
    {


        [Required(ErrorMessage = "Task Name is a required field")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The Task Name field cannot be shorter than 2 and longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is a required field")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AssignedToId should be greater than or equal to 0")]
        public int? AssignedToId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "ParentId should be greater than or equal to 0")]
        public int? ParentId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "SprintId should be greater than or equal to 0")]
        public int? SprintId { get; set; }

        [Required(ErrorMessage = "ProjectId  is required field")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectId should be greater than 0")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Task Type is required")]
        [Range(0, 4, ErrorMessage = "Task Type is invalid should be between 0-4")]
        public TaskTypes type { get; set; } = TaskTypes.Epic;

        [Required(ErrorMessage = "Task Status is required")]
        [Range(0, 2, ErrorMessage = "Status is invalid should be between 0-2")]
        public TasksStatus Status { get; set; } = TasksStatus.Pending;

        [Range(0, int.MaxValue, ErrorMessage = "Estimate hours cannot be less than 0")]
        public int EstimateHours { get; set; } = 0;


    }
}
