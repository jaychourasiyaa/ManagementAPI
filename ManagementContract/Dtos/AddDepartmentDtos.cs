using System.ComponentModel.DataAnnotations;

namespace ManagementAPI.Contract.Dtos
{
    public class AddDepartmentDtos
    {

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(35, MinimumLength = 2, ErrorMessage = "The Department Name field cannot be shorter than 2 characters longer than 15 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(\s+[a-zA-Z]+)*$", ErrorMessage = "The Name field can only contain alphabetic characters.")]
        public required string Name { get; set; }   
    }
}
