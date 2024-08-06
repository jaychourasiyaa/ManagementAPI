using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagementAPIEmployee;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementAPI.Contract.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public required int EmployeeId { get; set; }
        public DateTime date {  get; set; } = DateTime.Now;
        public int month { get; set; } = DateTime.Now.Month;
        public int year { get; set; } = DateTime.Now.Year;

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
    }
}
