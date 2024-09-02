using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Enums;
using ManagementAPIEmployee;
using System.ComponentModel.DataAnnotations.Schema;
namespace ManagementAPI.Contract.Models
{
    public  class Attendence
    {
        public int Id { get; set; }   
        public required int EmployeeId { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public required AttendenceStatus Status { get; set; } = AttendenceStatus.Present;

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
    }
}
