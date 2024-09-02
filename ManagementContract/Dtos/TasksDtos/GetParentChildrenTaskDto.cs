using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.TasksDtos
{
    public class GetParentChildrenTaskDto
    {
        [Required(ErrorMessage = "Project Id is a required fiedld")]
        [Range(1, int.MaxValue, ErrorMessage = "Project Id should be greater than 0")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Task Id is a required fiedld")]
        [Range(1, int.MaxValue, ErrorMessage = "Task Id should be greater than 0")]
        public int TaskId { get; set; }
        public bool WantChild { get; set; } = true;

    }
}
