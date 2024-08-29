using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;

namespace ManagementAPI.Contract.Dtos
{

   /* A task list will also have filters:
Sprint: single select.
Task type: multi-select.
Assigned to: multi-select (by default me must be selected)
Status: multi-select.*/
    public class TaskPaginatedDto
    {
       /* public string? filterOn { get; set; } = null;*/
        public string? filterQuery { get; set; } = null;

        /* public string ? additionalSearch { get; set; } = null;*/
        public string? SortBy { get; set; } = null;
        public bool IsAscending { get; set; } = true;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public DateTime? startDate { get; set; } = DateTime.MinValue;
        public DateTime? endDate { get; set; } = DateTime.MaxValue;
        public List<TasksStatus>? status { get; set; } = null;
        public List<TaskTypes>? type { get; set; } = null;
        public bool Assigned { get; set; } = true;
        public List<int> ? AssignedTo { get; set; } = null;

        [Range (0,int.MaxValue,ErrorMessage = "Sprint Id should be greater than or equal to 0")]
        public int? SprintId { get; set; } = null;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Project Id should be greater than 1")]
        public int ProjectID { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Parent Id should be greater than or equal to 0")]
        public int ?ParentId { get; set; }

    }
}
