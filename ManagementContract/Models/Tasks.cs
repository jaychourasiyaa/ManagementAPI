using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksReviewAPI ;
using Taask_status;
using ManagementAPI.Contract.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using ManagementAPIEmployee;
using ManagementAPI.Contract.Models;
using Microsoft.EntityFrameworkCore.Query;
namespace TasksAPI;

public class Tasks : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public required string Description { get; set; }

    
    public TasksStatus Status { get; set; } = TasksStatus.Pending;

    public bool IsActive { get; set; } = true;
   
   
    [ForeignKey(nameof(AssignedToId))]
    public  required int AssignedToId { get; set; }



    [ForeignKey(nameof(AssignedById))]
    public required int AssignedById { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public int ? ProjectId { get; set; } 
    public virtual ICollection<TasksReview> Reviews { get; set; }

    public Employee AssignedBy;
    public Employee AssignedTo;
    public Project Project;
   


}
