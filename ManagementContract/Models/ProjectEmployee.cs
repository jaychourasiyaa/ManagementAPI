using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Models
{
    public class ProjectEmployee
    {
        public int ProjectID { get; set; }
        [ForeignKey(nameof(ProjectID))]
        public Project Project { get; set; }
        public int EmployeeID { get; set; }
        [ForeignKey(nameof(EmployeeID))]
        public Employee Employee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
