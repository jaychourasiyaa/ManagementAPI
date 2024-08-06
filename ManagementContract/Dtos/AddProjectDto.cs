using ManagementAPI.Contract.Enums;
using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class AddProjectDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(\s+[a-zA-Z]+)*$", ErrorMessage = "The Name field can only contain alphabetic characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        
        [Required(ErrorMessage = "Project Creator Id is a required field")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee id should be greater than 0")]
        public int CreatedBy { get; set; } = 0;
        public ProjectStatus Status { get; set; } = ProjectStatus.Created;
        public List<int> Members { get; set; }  


    }
}
