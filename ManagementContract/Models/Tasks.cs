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
    public TaskTypes TaskType { get; set; } = TaskTypes.Epic;
    public TasksStatus Status { get; set; } = TasksStatus.Pending;
    public int EstimateHours { get; set; } = 0;
    public int RemainingHours { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(ParentId))]
    public int? ParentId { get; set; }

    [ForeignKey(nameof(AssignedToId))]
    public  int ? AssignedToId { get; set; }

    [ForeignKey(nameof(AssignedById))]
    public required int AssignedById { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public required int  ProjectId { get; set; } 

    [ForeignKey(nameof(SprintId))]
    public int ? SprintId  { get; set; }
    public virtual ICollection<TasksReview> Reviews { get; set; }

    public Employee AssignedBy;
    public Employee AssignedTo;
    public Project Project;
    public Tasks Parent;
    public Sprint Sprint;


}
