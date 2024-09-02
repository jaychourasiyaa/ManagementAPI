using ManagementAPI.Contract.Enums;
using ManagementAPI.Contract.Models;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taask_status;
using TasksAPI;

namespace ManagementAPI.Contract.Dtos.TasksDtos
{
    public class PreviousTaskValueDto
    {
        public int EHours;
        public int RHours;
        public string Description;
        public TaskTypes TaskType;
        public TasksStatus TaskStatus;
        public int? ParentId;
        public Tasks? Parent = null;
        public int? SprintId;
        public Sprint? Sprint = null;
        public int? AssignToId;
        public Employee? AssignTo = null;

    }
}
