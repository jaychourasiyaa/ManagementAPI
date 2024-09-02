using ManagementAPI.Contract.Enums;
using System.ComponentModel.DataAnnotations;

namespace ManagementAPI.Contract.Dtos.AttendenceDtos
{
    public class AddAttendenceDtos
    {
        [Required(ErrorMessage = "Employee Id is a required field")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee id should be greater than 0")]
        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Status is a required field")]
        [Range(0, 2, ErrorMessage = "Status value should be from 0-2")]
        public AttendenceStatus Status { get; set; } = AttendenceStatus.Present;
    }
}
