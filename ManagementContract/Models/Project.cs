using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIEmployee;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Enums;
using TasksAPI;
namespace ManagementAPI.Contract.Models
{
   public class Project : BaseEntity
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
      
        /*public DateTime CreatedAt { get; set; } = DateTime.Now;*/

        public ProjectStatus Status { get; set; } = ProjectStatus.Created;

        [ForeignKey(nameof(AssignedById))]
        public int AssignedById { get; set; } = 0;
        public Employee ProjectMaker {  get; set; }
        public virtual ICollection<ProjectEmployee> ProjectEmployee { get; set; }
        public virtual ICollection <Tasks> Taskss { get; set; }
       
       
    }
}
