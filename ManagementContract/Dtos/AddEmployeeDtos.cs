﻿using Employee_Role;
using System.ComponentModel.DataAnnotations;

namespace ManagementAPI.Contract.Dtos
{
    public class AddEmployeeDtos
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 20 characters.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 20 characters.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "The Name field cannot be shorter than 2 and longer than 15 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(\s+[a-zA-Z]+)*$", ErrorMessage = "The Name field can only contain alphabetic characters.")]
        public  string Name { get; set; }
        [Required ( ErrorMessage = "Salary is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary cannot be less than 1")]
        public  decimal Salary { get; set; }
        
        public int ? DepartmentId { get; set; }
       
        public int ? AdminId { get; set; }
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;

       
    }
}
