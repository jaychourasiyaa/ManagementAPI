using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.SalaryDtos
{
    public class SalaryDto
    {

        [Required(ErrorMessage = "Employee Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "EmloyeeId cannot be less than 0")]
        public int EmployeeId { get; set; }


    }
}
