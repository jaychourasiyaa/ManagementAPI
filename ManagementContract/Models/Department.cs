using ManagementAPI.Contract.Models;
using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPIDepartment;

public class Department : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(CreatedBy))]
    public Employee Employee { get; set; }
   /* public DateTime CreatedAt { get; set; } = DateTime.Now;*/
}
