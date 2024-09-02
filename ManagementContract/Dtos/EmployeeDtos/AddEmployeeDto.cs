using Employee_Role;
using System.ComponentModel.DataAnnotations;

namespace ManagementAPI.Contract.Dtos.EmployeeDtos
{
    public class AddEmployeeDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The UserName field cannot be shorter than 2 and longer than 20 characters.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password field should be of minimum 6 length")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 25  characters.")]
        [RegularExpression(@"^[a-zA-Z]+(\s+[a-zA-Z]+)*$", ErrorMessage = "The Name field can only contain alphabetic characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Salary is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary cannot be less than 1")]
        public decimal Salary { get; set; }

        public int? DepartmentId { get; set; }

        public int? AdminId { get; set; }
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;


    }
}
