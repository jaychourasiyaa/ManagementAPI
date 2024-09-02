using ManagementAPI.Contract.Enums;
using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.ProjectDtos
{
    public class AddProjectDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 100 characters.")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }


        [Range(0, 2, ErrorMessage = "Status should be within 0-2")]
        public ProjectStatus Status { get; set; } = ProjectStatus.Created;
        public List<int> Members { get; set; } = new List<int>();


    }
}
