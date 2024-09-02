using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.SprintDtos
{
    public class AddSprintDto
    {
        [Required(ErrorMessage = "Sprint Name is a Required Feild")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Project Id is required Feild")]
        [Range(0, int.MaxValue, ErrorMessage = "ProjectId must be greater than 0")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "startDate is required Feild")]
        public DateTime startDate { get; set; }
        [Required(ErrorMessage = "endDate Id is required Feild")]
        public DateTime endDate { get; set; }
    }
}
