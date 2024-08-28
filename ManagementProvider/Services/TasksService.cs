using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using TasksAPI;
using ManagementAPI.Contract.Dtos;
using Microsoft.EntityFrameworkCore;
using ManagementAPIEmployee;
using ManagementAPI.Provider.Migrations;
using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ManagementAPI.Contract.Enums;
using ManagementAPI.Contract.Responses;
using Employee_Role;
using Microsoft.Identity.Client;
using Taask_status;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ManagementAPI.Contract.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Runtime.Versioning;
using System.ComponentModel.DataAnnotations;
using System.Data;
namespace ManagementAPI.Provider.Services
{
    public class TasksService : ITasksServices
    {
        private readonly dbContext _dbContext;

        public TasksService(dbContext db)
        {
            _dbContext = db;

        }
        public IQueryable<GetTaskDto>? ApplyFiltering(IQueryable<GetTaskDto>? tasks, TaskPaginatedDto PDto,
            int assingedTo)
        {
            string? filterQuery = PDto.filterQuery;
            List<TasksStatus>? Status = PDto.status;
            List<TaskTypes>? Type = PDto.type;
            List<int?>? AssignedTo = PDto.AssignedTo.Where(e => e != 0).ToList();
            bool assigned = PDto.Assigned;
            int? SprintId = PDto.SprintId;
            int? ParentId = PDto.ParentId;
            DateTime? startDate = PDto.startDate;
            DateTime? endDate = PDto.endDate;

            if (startDate != null && endDate != null)
            {
                tasks = tasks.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate);
            }
            if (assigned)
            {
                tasks = tasks.Where(t => t.AssignedToId != null);
            }
            else
            {
                tasks = tasks.Where(t => t.AssignedToId == null);
            }
            if (SprintId != null && SprintId != 0)
            {
                tasks = tasks.Where(t => t.SprintId == SprintId);
            }
            if (ParentId != null && ParentId != 0)
            {
                tasks = tasks.Where(t => t.ParentId == ParentId);
            }
            if (Status != null && Status.Count != 0)
            {
                tasks = tasks.Where(t => Status.Contains(t.Status));

            }

            if (Type != null && Type.Count != 0)
            {
                tasks = tasks.Where(t => Type.Contains(t.Type));
            }
            if (AssignedTo.Count != 0)
            {
                tasks = tasks.Where(t => AssignedTo.Contains(t.AssignedToId));
            }
            if (string.IsNullOrEmpty(filterQuery) == false)
            {
                if (filterQuery != "")
                {
                    tasks = tasks.Where(e => e.Name.Contains(filterQuery));
                }
            }
            return tasks;

            ////
            ////
            ////
            ////
            /*if (string.IsNullOrEmpty(filterQuery) == false)
            {
                // filer according to name or subpart of name
                if (filterQuery != "" )
                {
                    tasks = tasks.Where(e => e.Name.Contains(filterQuery));
                }

                // filter according to department or subpart of department name
                *//*else if (filterOn.Equals("Assigned_From", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = tasks.Where(e => e.Assigned_From.Contains(filterQuery));
                }
                else if (filterOn.Equals("Assigned_To", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = tasks.Where(e => e.Assigned_To.Contains(filterQuery));
                }*//*
                else if (filterOn.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    // Checking value of filterQuery is number or not
                    if (int.TryParse(filterQuery, out var statusId))
                    {
                        // if number then check for valid number from 0-2
                        if (Enum.IsDefined(typeof(TasksStatus), statusId))
                        {
                            var status = (TasksStatus)statusId;
                            tasks = tasks.Where(e => e.Status == status);
                        }
                        else
                        {
                            throw new Exception("Invalid Status ID specified.");
                        }
                    }
                    else
                    {
                        // Normalize filterQuery to match enum names case insensitively
                        var normalizedFilterQuery = filterQuery.Trim().ToLowerInvariant();

                        var matchingStatus = Enum.GetValues(typeof(TasksStatus))
                            .Cast<TasksStatus>()
                            .FirstOrDefault(role => role.ToString().ToLowerInvariant() == normalizedFilterQuery);

                        *//* if (matchingStatus != default(ProjectStatus))
                         {*//*
                        tasks = tasks.Where(e => e.Status == matchingStatus);
                        *//*}
                        else
                        {
                            throw new Exception("Invalid Status name specified.");
                        }*//*
                    }
                }


            }
            return tasks;*/
        }

        public IQueryable<GetTaskDto>? ApplySorting(IQueryable<GetTaskDto>? tasks, string? SortBy,
            bool IsAscending)
        {
            if (string.IsNullOrEmpty(SortBy) == false)
            {
                if (SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {

                    tasks = IsAscending ? tasks.OrderBy(e => e.Name) : tasks.OrderByDescending(e => e.Name);

                }
                if (SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = IsAscending ? tasks.OrderBy(e => e.CreatedOn) :
                    tasks.OrderByDescending(e => e.CreatedOn);

                }
                if (SortBy.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = IsAscending ? tasks.OrderBy(e => e.Status) :
                        tasks.OrderByDescending(e => e.Status);
                }

                if (SortBy.Equals("Type", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = IsAscending ? tasks.OrderBy(e => e.Type) :
                        tasks.OrderByDescending(e => e.Type);
                }
                if (SortBy.Equals("AssignedBy", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = IsAscending ? tasks.OrderBy(e => e.AssignedById) :
                        tasks.OrderByDescending(e => e.AssignedById);
                }
                if (SortBy.Equals("AssingedTo", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = IsAscending ? tasks.OrderBy(e => e.AssignedToId) :
                        tasks.OrderByDescending(e => e.AssignedToId);
                }



            }
            return tasks;
        }

        public async Task<bool> CheckManagerOfEmployee(int managerId, int? employeeId)
        {
            var manager = await _dbContext.Employees
                .Where(e => e.Id == employeeId && e.AdminId == managerId)
                .FirstOrDefaultAsync();
            if (manager == null) return false;
            return true;
        }


        //Member1 = Task Assigner
        //Member2 = Task Assignee
        public async Task<bool> CheckTeamMemberOfProject(int Member1Id, int? Member2Id, int ProjectId)
        {
            try
            {
                var Member1Exists = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == ProjectId && p.EmployeeID == Member1Id);
                if (Member1Exists == null)
                {
                    return false;
                }
                var Member2Exists = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == ProjectId && p.EmployeeID == Member2Id);
                if (Member2Exists == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(int, List<GetTaskDto>)> GetAllTasks(EmployeeRole Role, int assignedTo, TaskPaginatedDto PDto)
        {
            try
            {


                string? SortBy = PDto.SortBy;
                bool IsAscending = PDto.IsAscending;
                int pageNumber = PDto.pageNumber == 0 ? 1 : PDto.pageNumber;
                int pageSize = PDto.pageSize == 0 ? 10 : PDto.pageSize;
                int ProjectId = PDto.ProjectID;
                List<int?>? AssignedTo = PDto.AssignedTo.Where(e => e != 0).ToList();
                int count = 0;
                var tasks = _dbContext.Taasks.Include(e => e.AssignedBy)
                    .Include(e => e.AssignedTo)
                    .Where(e => e.ProjectId == ProjectId && e.IsActive == true)
                 .Select(e => new GetTaskDto
                 {
                     Id = e.Id,
                     Name = e.Name,
                     AssignedById = e.AssignedById,
                     AssignedToId = e.AssignedToId,
                     Assigned_From = e.AssignedBy.Name,
                     Assigned_To = e.AssignedTo.Name,
                     CreatedOn = e.CreatedOn,
                     Description = e.Description,
                     Status = e.Status,
                     ParentId = e.ParentId,
                     ProjectId = e.ProjectId,
                     SprintId = e.SprintId,
                     Type = e.TaskType

                 }).AsQueryable();
                count = await tasks.CountAsync();



                if (Role != EmployeeRole.SuperAdmin)
                {
                    tasks = tasks.Where(t => t.AssignedToId == assignedTo);
                    /*tasks = tasks.Where(t => t.AssignedById == assignedTo || t.AssignedToId == assignedTo);*/
                    count = await tasks.CountAsync();

                }
                tasks = ApplyFiltering(tasks, PDto, assignedTo);
                tasks = ApplySorting(tasks, SortBy, IsAscending);
                var skipResult = (pageNumber - 1) * pageSize;
                return (count, await tasks.Skip(skipResult).Take(pageSize).ToListAsync());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> checkTaskHeirarchy(int? taskId, TaskTypes type, int ParentId, int ProjectId)
        {
            try
            {
                if (type == TaskTypes.Feature)
                {
                    var checkEpic = await _dbContext.Taasks.Where(t => t.Id == ParentId && t.TaskType == TaskTypes.Epic && t.ProjectId == ProjectId).FirstOrDefaultAsync();
                    if (checkEpic == null)
                    {
                        return -7;
                    }
                    if (taskId != null && checkEpic.Id == taskId)
                    {
                        return -99;
                    }

                }
                else if (type == TaskTypes.UserStory)
                {
                    var checkFeature = await _dbContext.Taasks.Where(t => t.Id == ParentId && t.TaskType == TaskTypes.Feature && t.ProjectId == ProjectId).FirstOrDefaultAsync();
                    if (checkFeature == null)
                    {
                        return -8;
                    }
                    if (taskId != null && checkFeature.Id == taskId)
                    {
                        return -99;
                    }
                }
                else if (type == TaskTypes.Task)
                {
                    var checkUserStory = await _dbContext.Taasks.Where(t => t.Id == ParentId && t.TaskType == TaskTypes.UserStory && t.ProjectId == ProjectId).FirstOrDefaultAsync();
                    if (checkUserStory == null)
                    {
                        return -9;
                    }
                    if (taskId != null && checkUserStory.Id == taskId)
                    {
                        return -99;
                    }
                }
                else if (type == TaskTypes.Bugs)
                {
                    var checkUserStory = await _dbContext.Taasks.Where(t => t.Id == ParentId && t.TaskType == TaskTypes.UserStory && t.ProjectId == ProjectId).FirstOrDefaultAsync();
                    if (checkUserStory == null)
                    {
                        return -10;
                    }
                    if (taskId != null && checkUserStory.Id == taskId)
                    {
                        return -99;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> CheckInputDetails(AddTasksDtos dtos, int assignedBy)
        {
            try
            {
                if (dtos.AssignedToId == 0) { dtos.AssignedToId = null; }
                if (dtos.ParentId == 0) { dtos.ParentId = null; }
                if (dtos.SprintId == 0) { dtos.SprintId = null; }

                if (dtos.type == TaskTypes.Task || dtos.type == TaskTypes.Bugs)
                {
                    if (dtos.ParentId == null)
                    {
                        return -1;
                    }
                }
                var manager = await _dbContext.Employees.Where(e => e.Id == assignedBy).FirstOrDefaultAsync();
                if (manager == null || !manager.IsActive)
                {
                    return -2;
                }
                var project = await _dbContext.Projects
                   .Where(p => p.Id == dtos.ProjectId)
                   .FirstOrDefaultAsync();
                if (project == null)
                {
                    return -3;
                }
                if (dtos.AssignedToId != null)
                {
                    var employee = await _dbContext.Employees.Where(e => e.Id == dtos.AssignedToId).FirstOrDefaultAsync();
                    if (employee == null || !employee.IsActive)
                    {
                        return -4;
                    }
                    var projectEmployee = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(e => e.ProjectID == project.Id && e.EmployeeID == employee.Id);
                    if (projectEmployee == null)
                    {
                        return -5;
                    }

                    // if the task is not self assigned
                    // then
                    // to assign a task one should be superadmin 
                    // or manager of a project team member
                    // or member of same project 
                    if (dtos.AssignedToId != null)
                    {
                        if (manager.Role != EmployeeRole.SuperAdmin && dtos.AssignedToId != assignedBy)
                        {
                            bool checkManager = await CheckManagerOfEmployee(manager.Id, employee.Id);
                            bool checkTeamMember = await CheckTeamMemberOfProject(assignedBy, dtos.AssignedToId, project.Id);


                            if (!checkManager && !checkTeamMember)
                            {
                                return -6;
                            }

                        }
                    }
                }

                if (dtos.type == TaskTypes.Epic)
                {
                    dtos.ParentId = null;
                }
                else if (dtos.ParentId != null)// (dtos.type != TaskTypes.Epic)
                {
                    int ParentId = Convert.ToInt32(dtos.ParentId);
                    int checkTaskLevel = await checkTaskHeirarchy(null, dtos.type, ParentId, project.Id);
                    if (checkTaskLevel < 0)
                    {
                        return checkTaskLevel;
                    }
                    /* if (dtos.type == TaskTypes.Feature)
                     {
                         var checkEpic = await _dbContext.Taasks.Where(t => t.Id == dtos.ParentId && t.TaskType == TaskTypes.Epic && t.ProjectId == project.Id).FirstOrDefaultAsync();
                         if (checkEpic == null)
                         {
                             return -4;
                         }
                     }
                     else if (dtos.type == TaskTypes.UserStory)
                     {
                         var checkFeature = await _dbContext.Taasks.Where(t => t.Id == dtos.ParentId && t.TaskType == TaskTypes.Feature && t.ProjectId == project.Id).FirstOrDefaultAsync();
                         if (checkFeature == null)
                         {
                             return -5;
                         }
                     }
                     else if (dtos.type == TaskTypes.Task)
                     {
                         var checkUserStory = await _dbContext.Taasks.Where(t => t.Id == dtos.ParentId && t.TaskType == TaskTypes.UserStory && t.ProjectId == project.Id).FirstOrDefaultAsync();
                         if (checkUserStory == null)
                         {
                             return -6;
                         }
                     }
                     else if (dtos.type == TaskTypes.Bugs)
                     {
                         var checkUserStory = await _dbContext.Taasks.Where(t => t.Id == dtos.ParentId && t.TaskType == TaskTypes.UserStory && t.ProjectId == project.Id).FirstOrDefaultAsync();
                         if (checkUserStory == null)
                         {
                             return -7;
                         }
                     }*/
                }


                if (dtos.SprintId != null)
                {
                    var sprint = await _dbContext.Sprints.FirstOrDefaultAsync(s => s.Id == dtos.SprintId
                                        && s.ProjectId == project.Id);
                    if (sprint == null)
                    {
                        return -11;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddTasks(AddTasksDtos dtos, int assignedBy)
        {
            try
            {
                int checkInput = await CheckInputDetails(dtos, assignedBy);
                if (checkInput < 0)
                {
                    return checkInput;
                }

                var tasks = new Tasks
                {
                    Name = dtos.Name,
                    AssignedById = assignedBy,
                    AssignedToId = dtos.AssignedToId,
                    Description = dtos.Description,
                    TaskType = dtos.type,
                    Status = dtos.Status,
                    CreatedBy = assignedBy,
                    ParentId = dtos.ParentId,
                    ProjectId = dtos.ProjectId,
                    SprintId = dtos.SprintId,
                    EstimateHours = dtos.EstimateHours,
                    RemainingHours = dtos.EstimateHours
                };
                await _dbContext.Taasks.AddAsync(tasks);
                await _dbContext.SaveChangesAsync();
                return tasks.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> CheckTaskAccess(EmployeeRole Role, Tasks tasks, int AccessingId, Project project)
        {
            try
            {
                bool Assignee = false;
                bool checkManager = false;
                bool checkTeamMember = false;
                bool Assigner = false;
                if (Role != EmployeeRole.SuperAdmin) // not superadmin accessing
                {
                    if (tasks.AssignedToId != null) // for assigned task check for assigneer , assignee, manager of assignee 
                    {
                        if (tasks.AssignedById == AccessingId) // check for assinger
                        {
                            Assigner = true;
                        }
                        else if (tasks.AssignedToId == AccessingId) // check for assignee
                        {
                            Assignee = true;
                        }
                        else // (tasks.AssignedById != AccessingId && tasks.AssignedToId != AccessingId) // checking for manager of assignee
                        {
                            checkManager = await CheckManagerOfEmployee(AccessingId, tasks.AssignedToId);
                        }
                    }
                    else // unassigned task
                    {
                        if (tasks.AssignedById == AccessingId) // task has no assigne so no nedd to check for manager just check for assigner
                        {
                            Assigner = true;
                        }
                    }

                    // if task is not accessed by assinger , assingee , manager of assgnee, need to check for project team member
                    if (!checkManager && !Assigner && !Assignee)
                    {
                        checkTeamMember = await CheckTeamMemberOfProject(AccessingId, tasks.AssignedToId, project.Id);
                    }
                    if (!checkManager && !Assigner && !checkTeamMember && !Assignee) // task is unaccessible
                    {
                        return -6;
                    }
                }
                return 1;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateTasks(UpdateTasksDto dto, int id, int AccessingId, EmployeeRole Role)
        {
            try
            {

                // checks that does not reqiure calling database
                if (dto.type != null)
                {
                    //task type epic cannot have parent
                    if (dto.type == TaskTypes.Epic && dto.ParentId != null)
                    {
                        return -1;
                    }
                    //task type task and bugs must need a parent
                    else if (dto.type == TaskTypes.Task || dto.type == TaskTypes.Bugs)
                    {
                        if (dto.ParentId == null)
                        {
                            return -2;
                        }
                    }
                }
                // checking that the legit person is logged in
                var Accessor = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == AccessingId && e.IsActive);
                if (Accessor == null)
                {
                    return -3;
                }

                //checking valid project 
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
                if (project == null)
                {
                    return -4;
                }

                // checking a valid task or not with given task Id and project Id
                var tasks = await _dbContext.Taasks.Where(e => e.Id == id && e.IsActive && e.ProjectId == project.Id).FirstOrDefaultAsync();
                if (tasks == null || !tasks.IsActive)
                {
                    return -5;
                }

                // storing previous value for comparison after updation to generate log
                int previousEHours = tasks.EstimateHours;
                int previousRHours = tasks.RemainingHours;
                string previousDescription = tasks.Description;
                int? previousParentId = tasks.ParentId;
                TaskTypes previousTaskType = tasks.TaskType;
                TasksStatus previousTaskStatus = tasks.Status;
                var PreviousParent = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasks.ParentId);
                var previousSprint = await _dbContext.Sprints.FirstOrDefaultAsync(s => s.Id == tasks.SprintId);
                int? previousSprintId = tasks.SprintId;
                var previousAssignTo = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == tasks.AssignedToId);
                int? previousAssignToId = tasks.AssignedToId;
                // if valid task found checking accessibility 
                // Who can access the task : 
                // -- a user who has assigned task
                // -- or a user whom the task has been assigned
                // -- or superadmin
                // -- or project team member 
                // -- or the manager of the user whom the task has been assigned 
                // all the above have the access the update the task

                int checkAccess = await CheckTaskAccess(Role, tasks, AccessingId, project);
                if(checkAccess <0)
                {
                    return checkAccess;
                }
                
                // if task is accessible
                // if user want to task type ->parent id is needed expect if task type is epic
                if (dto.type != null)
                {
                    if (dto.type == TaskTypes.Epic)
                    {
                        tasks.TaskType = TaskTypes.Epic;
                        tasks.ParentId = null;
                    }
                    else
                    {
                        var type = (TaskTypes)dto.type;
                        int ParentId = Convert.ToInt32(dto.ParentId);
                        if (dto.ParentId != null)
                        {
                            int checkTaskLevel = await checkTaskHeirarchy(tasks.Id, type, ParentId, project.Id);
                            if (checkTaskLevel < 0)
                            {
                                return checkTaskLevel;
                            }
                        }
                        tasks.ParentId = dto.ParentId;
                        tasks.TaskType = (TaskTypes)dto.type;
                    }
                }

                // checking if user want to update estimated and remaining hours or not 
                // if yes giving checks that remaining hours must be less than estimated hours
                if (dto.EstimateHours != null && dto.RemainingHours != null)
                {
                    if (dto.RemainingHours > dto.EstimateHours)
                    {
                        return -11;
                    }
                    tasks.EstimateHours = Convert.ToInt32(dto.EstimateHours);
                    tasks.RemainingHours = Convert.ToInt32(dto.RemainingHours);
                }
                else if (dto.EstimateHours != null)
                {
                    if (dto.EstimateHours < tasks.RemainingHours)
                    {
                        return -11;
                    }
                    tasks.EstimateHours = Convert.ToInt32(dto.EstimateHours);

                }
                else if (dto.RemainingHours != null)
                {
                    if (dto.RemainingHours > tasks.EstimateHours)
                    {
                        return -11;
                    }
                    tasks.RemainingHours = Convert.ToInt32(dto.RemainingHours);

                }
                if (dto.Description != null)
                {
                    tasks.Description = dto.Description;
                }
                // if user want to update parent tasks
                if (dto.type == null && dto.ParentId != null)
                {
                    var parentTask = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == dto.ParentId && t.IsActive);

                    if (parentTask == null)
                    {
                        return -12;
                    }
                    var checkTaskLevel = await checkTaskHeirarchy(tasks.Id, tasks.TaskType, parentTask.Id, project.Id);
                    if (checkTaskLevel < 0)
                    {
                        return checkTaskLevel;
                    }
                    tasks.ParentId = parentTask.Id;
                }
                int status = Convert.ToInt32(dto.Status);
                if (status == 0)
                {
                    tasks.Status = TasksStatus.Pending;
                }
                else if (status == 1)
                {
                    tasks.Status = TasksStatus.Running;
                }

                else
                {
                    tasks.Status = TasksStatus.Completed;
                }
                if (dto.SprintId != null)
                {
                    var sprint = await _dbContext.Sprints.Where(s => s.Id == dto.SprintId && s.ProjectId == tasks.ProjectId).FirstOrDefaultAsync();
                    if (sprint == null)
                    {
                        return -13;
                    }
                    tasks.SprintId = sprint.Id;
                }
                if (dto.AssignTo != null)
                {
                    var assignTo = await _dbContext.Employees.Where(e => e.Id == dto.AssignTo).FirstOrDefaultAsync();
                    if (assignTo == null)
                    {
                        return -14;
                    }
                    var checkProjectMember = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == project.Id && p.EmployeeID == assignTo.Id);
                    if (checkProjectMember == null)
                    {
                        return -14;
                    }
                    tasks.AssignedToId = assignTo.Id;
                }
                
                //
                //
                // Adding Logs for changes
                if (previousEHours != tasks.EstimateHours)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {Accessor.Name} Changed Estimated Hours from {previousEHours} to {tasks.EstimateHours} "
                    };
                    _dbContext.Logs.Add(log);
                }
                if (previousRHours != tasks.RemainingHours)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {Accessor.Name} Changed Remaining Hours from {previousRHours} to {tasks.RemainingHours} "
                    };
                    _dbContext.Logs.Add(log);
                }
                if (previousTaskType != tasks.TaskType)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {Accessor.Name} Changed Task Type from {previousTaskType.ToString()}  to {tasks.TaskType.ToString()}"
                    };
                    _dbContext.Logs.Add(log);
                }
                var UpdatedParent = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasks.ParentId);
                if (previousParentId != tasks.ParentId)
                {

                    if (PreviousParent != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Changed Parent Task from {PreviousParent.Id} : {PreviousParent.Name} to {UpdatedParent.Id}" +
                            $": {UpdatedParent.Name} "
                        };
                        _dbContext.Logs.Add(log);
                        
                    }
                    else
                    {
                        
                            var log = new Log
                            {
                                TaskId = tasks.Id,
                                Message = $" {Accessor.Name} Added Parent Task  {UpdatedParent.Id}" +
                                $": {UpdatedParent.Name} "
                            };
                            _dbContext.Logs.Add(log);
                        
                    }
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Added Child Task {tasks.Id} : {tasks.Name}" +
                            $" in {UpdatedParent.Id} " +
                                $": {UpdatedParent.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                }
                if (previousDescription != tasks.Description)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {Accessor.Name} Changed Description from {previousDescription}  to {tasks.Description}"
                    };
                    _dbContext.Logs.Add(log);
                }
                if (previousTaskStatus != tasks.Status)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {Accessor.Name} Changed Status from {previousTaskStatus.ToString()}  to {tasks.Status.ToString()}"
                    };
                    _dbContext.Logs.Add(log);
                }
                if (previousAssignToId != tasks.AssignedToId)
                {
                    var updatedAssignTo = await _dbContext.Employees.Where(e => e.Id == tasks.AssignedToId).FirstOrDefaultAsync();
                    if (previousAssignTo != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Changed AssingTo from {previousAssignToId} : {previousAssignTo.Name} to {updatedAssignTo.Id}" + $": {updatedAssignTo.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                    else
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Added AssignTo  {updatedAssignTo.Id}" +
                            $": {updatedAssignTo.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                }
                if (previousSprintId != tasks.SprintId)
                {
                    var Updatedsprint = await _dbContext.Sprints.Where(s => s.Id == tasks.SprintId).FirstOrDefaultAsync();
                    if (previousSprint != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Changed Sprint from {previousSprintId} : {previousSprint.Name} to {Updatedsprint.Id}" + $": {Updatedsprint.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                    else
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {Accessor.Name} Added Srpint  {Updatedsprint.Id}" +
                            $": {Updatedsprint.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                }
                await _dbContext.SaveChangesAsync();
                return tasks.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteTasks(int id, int AccessingId, EmployeeRole role)
        {
            try
            {
                var tasks = await _dbContext.Taasks.FirstOrDefaultAsync(e => e.Id == id);
                if (tasks == null || !tasks.IsActive) return false;
                if (tasks.AssignedById != AccessingId && role != EmployeeRole.SuperAdmin) return false;

                tasks.IsActive = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<GetTaskByIdDto>?> GetTaskById(int assigneToId)
        {
            try
            {
                var tasksList = await _dbContext.Taasks
                    .Where(t => t.AssignedToId == assigneToId && t.IsActive)
                    .Select(e => new GetTaskByIdDto
                    {

                        Name = e.Name,
                        Description = e.Description,
                        Assigned_From = e.AssignedBy.Name,
                        Assigned_To = e.AssignedTo.Name,
                        CreatedOn = e.CreatedOn,
                        Status = e.Status,



                    }).ToListAsync();
                if (tasksList == null) return null;
                return tasksList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(int, List<GetTaskDto>?)> getTasks(int projectId, int taskId, bool children)
        {
            try
            {

                List<GetTaskDto>? resultTasks = new List<GetTaskDto>();
                var project = await _dbContext.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();
                if (project == null)
                {
                    return (-1, null);
                }
                var tasks = await _dbContext.Taasks.Where(t => t.Id == taskId && t.ProjectId == projectId).FirstOrDefaultAsync();

                if (tasks == null)
                {
                    return (-2, null);
                }

                int count = 0;
                var type = tasks.TaskType;
                var toFindType = new TaskTypes();
                bool flag = true;
                var query = _dbContext.Taasks.Where(t => t.ProjectId == projectId)
                            .Select(t => new GetTaskDto
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Description = t.Description,
                                AssignedById = t.AssignedById,
                                AssignedToId = t.AssignedToId,
                                Assigned_From = t.AssignedBy.Name,
                                Assigned_To = t.AssignedTo.Name,
                                CreatedOn = t.CreatedOn,
                                ParentId = t.ParentId,
                                ProjectId = t.ProjectId,
                                Status = t.Status,
                                Type = t.TaskType,
                            }).AsQueryable();
                count = resultTasks.Count;

                if (children && type != TaskTypes.Task && type != TaskTypes.Bugs) // want children
                {
                    toFindType = (TaskTypes)(Convert.ToInt32(type) + 1);
                    if (type != TaskTypes.UserStory)
                    {
                        query = query.Where(t => t.Type == toFindType);
                        flag = false;

                    }
                    else
                    {
                        query = query.Where(t => t.Type == toFindType || t.Type == TaskTypes.Bugs);
                        flag = false;
                    }
                }
                else if (children == false && type != TaskTypes.Epic)// want parent
                {

                    toFindType = (TaskTypes)(Convert.ToInt32(type) - 1);
                    if (type == TaskTypes.Task || type == TaskTypes.Bugs)
                    {
                        query = query.Where(t => t.Type == TaskTypes.UserStory);
                        flag = false;
                    }
                    else
                    {
                        query = query.Where(t => t.Type == toFindType);
                        flag = false;
                    }
                }
                if (flag)
                {
                    return (0, null);
                }
                resultTasks = await query.ToListAsync();
                count = query.Count();
                return (count, resultTasks);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
