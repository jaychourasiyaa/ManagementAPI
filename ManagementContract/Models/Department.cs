using ManagementAPI.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPIDepartment;

public class Department : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
   /* public DateTime CreatedAt { get; set; } = DateTime.Now;*/
}
