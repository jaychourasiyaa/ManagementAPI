using ManagementAPI.Contract.Enums;
using System.ComponentModel.DataAnnotations;

namespace ManagementAPI.Contract.Dtos
{
    public class AddAttendenceDtos
    {
        [Required (ErrorMessage = "Employee Id is a required field")]
        [Range (1,int.MaxValue ,ErrorMessage = "Employee id should be greater than 0")]
        public int? EmployeeId { get; set; }
        [Required (ErrorMessage = "Status is a required field")]
        [Range (1,3 , ErrorMessage ="Status value should be from 1-3")]
        public AttendenceStatus Status { get; set; } = AttendenceStatus.Present;
    }
}
