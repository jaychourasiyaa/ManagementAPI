using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using TasksAPI;
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
using System.Net.Http.Headers;
using ManagementAPI.Contract.Dtos.TasksDtos;
namespace ManagementAPI.Provider.Services
{
    public class TasksService : ITasksServices
    {
        private readonly dbContext _dbContext;
        private readonly ISortingService SortingService;
        private readonly IJwtService JwtService;
        public TasksService(dbContext db, ISortingService sortingservice, IJwtService jwtService)
        {
            _dbContext = db;
            SortingService = sortingservice;
            JwtService = jwtService;
        }
        public IQueryable<GetTaskDto>? ApplyFiltering(IQueryable<GetTaskDto>? tasks, TaskPaginatedDto PDto,
            int assignedTo)
        {
            // if tasks query is null not need to filter
            if (tasks == null)
            {
                return tasks;
            }

            // filter accordint to status 
            if (PDto.status == null)
            {
                PDto.status = new List<TasksStatus>();
                PDto.status.Add(TasksStatus.Running); // if status is null be default giving only running task
                if (PDto.ParentId != null)
                {
                    PDto.status.Add(TasksStatus.Pending);
                    PDto.status.Add(TasksStatus.Completed);
                }
            }
            tasks = tasks.Where(t => PDto.status.Contains(t.Status));

            // filter accordint to type
            if (PDto.type == null)
            {
                PDto.type = new List<TaskTypes>();
                PDto.type.Add(TaskTypes.Epic); // if type is null by default giving all epic
                if (PDto.ParentId != null)
                {
                    PDto.type.Add(TaskTypes.Feature);
                    PDto.type.Add(TaskTypes.UserStory);
                    PDto.type.Add(TaskTypes.Task);
                    PDto.type.Add(TaskTypes.Bugs);
                }
            }
            tasks = tasks.Where(t => PDto.type.Contains(t.Type));

            //filter according to assigned to team member
            List<int>? AssignedTo = PDto.AssignedTo != null ? PDto.AssignedTo.Where(e => e != 0).ToList() : null;
            if (AssignedTo == null)
            {
                AssignedTo = new List<int>();
                AssignedTo.Add(assignedTo);// by default logged in users tasks
            }
            if (JwtService.UserRole != EmployeeRole.SuperAdmin)
            {
                tasks = tasks.Where(t => t.AssignedToId != null && AssignedTo.Contains((t.AssignedToId.Value)));
            }

            //date range filter search
            if (PDto.startDate != null && PDto.endDate != null)
            {
                tasks = tasks.Where(t => t.CreatedOn >= PDto.startDate && t.CreatedOn <= PDto.endDate);
            }

            // assigned task search
            if (PDto.Assigned)
            {
                tasks = tasks.Where(t => t.AssignedToId != null);
            }
            else // unassigned task search
            {
                tasks = tasks.Where(t => t.AssignedToId == null);
            }

            // Filter accordint to a particular sprint
            if (PDto.SprintId != null)
            {
                tasks = tasks.Where(t => t.SprintId == PDto.SprintId);
            }

            //filter accordint to particular parent
            if (PDto.ParentId != null)
            {
                tasks = tasks.Where(t => t.ParentId == PDto.ParentId);
            }

            // search on task name only
            if (string.IsNullOrEmpty(PDto.filterQuery) == false)
            {
                if (PDto.filterQuery != "")
                {
                    tasks = tasks.Where(e => e.Name.Contains(PDto.filterQuery));
                }
            }
            return tasks;
        }
        public async Task<bool> CheckTeamMemberOfProject(int Member1Id, int? Member2Id, int ProjectId)
        {
            try
            {
                var Member1Exists = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == ProjectId && p.EmployeeID == Member1Id);
                if (Member1Exists == null)
                {
                    return false;
                }
                if (Member2Id != null)
                {
                    var Member2Exists = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == ProjectId && p.EmployeeID == Member2Id);
                    if (Member2Exists == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> CheckManagerOfEmployee(int managerId, int? employeeId)
        {
            var manager = await _dbContext.Employees
                .Where(e => e.Id == employeeId && e.AdminId == managerId)
                .FirstOrDefaultAsync();
            if (manager == null) return false;
            return true;
        }
        public async Task<int> CheckTaskHeirarchy(int? taskId, TaskTypes type, int ParentId, int ProjectId)
        {
            try
            {

                // heirarchy rule
                // -- Epic cannot have parent , task (normal) and bugs must have parent
                // -- feature and userStory may or may not have parent
                // -- feature must have parent of type epic
                // -- userStory must have parent of type feature
                // -- task and bugs must have parent of type user Story

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
        public async Task<int> CheckInputDetails(AddTasksDto dtos, int assignedBy)
        {
            try
            {
                if (dtos.AssignedToId == 0) { dtos.AssignedToId = null; }
                if (dtos.ParentId == 0) { dtos.ParentId = null; }
                if (dtos.SprintId == 0) { dtos.SprintId = null; }

                // to create normal task or bugs parent is required
                if (dtos.type == TaskTypes.Task || dtos.type == TaskTypes.Bugs)
                {
                    if (dtos.ParentId == null)
                    {
                        return -1;
                    }
                }

                // checking person who is assigning task is valid or not
                var manager = await _dbContext.Employees.Where(e => e.Id == assignedBy).FirstOrDefaultAsync();
                if (manager == null || !manager.IsActive)
                {
                    return -2;
                }

                //checking project under which task is being made is valid project or not
                var project = await _dbContext.Projects
                   .Where(p => p.Id == dtos.ProjectId)
                   .FirstOrDefaultAsync();
                if (project == null)
                {
                    return -3;
                }

                // if task is being assigned to someone need to check some requirements
                if (dtos.AssignedToId != null)
                {
                    // checking the person is valid or not whom the task is being to assign
                    var employee = await _dbContext.Employees.Where(e => e.Id == dtos.AssignedToId).FirstOrDefaultAsync();
                    if (employee == null || !employee.IsActive)
                    {
                        return -4;
                    }

                    // checking that the employee is memeber or project or not
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

                // task type epic cannot have parent
                if (dtos.type == TaskTypes.Epic)
                {
                    dtos.ParentId = null;
                }
                else if (dtos.ParentId != null)// (dtos.type != TaskTypes.Epic)
                {
                    // checking for the heirarchy creiteria of task with
                    int ParentId = Convert.ToInt32(dtos.ParentId);
                    int checkTaskLevel = await CheckTaskHeirarchy(null, dtos.type, ParentId, project.Id);
                    if (checkTaskLevel < 0)
                    {
                        return checkTaskLevel;
                    }
                }

                //checking if task is being created under a sprint 
                // if yes sprint must be aligned with project
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateTaskDetails(Tasks tasks, UpdateTasksDto dto, int projectId)
        {
            try
            {
                // if user want to task type then, ->parent id is needed expect if task type is epic
                if (dto.type != null)
                {
                    // updating task type to epic make its parent id as null
                    if (dto.type == TaskTypes.Epic)
                    {
                        tasks.TaskType = TaskTypes.Epic;
                        tasks.ParentId = null;
                    }
                    else
                    {
                        // updating to feature , user story , task ,bugs
                        // task and bugs must required parent Id already checked above
                        // feature and user story have parent id optional

                        var type = (TaskTypes)dto.type;
                        int ParentId = Convert.ToInt32(dto.ParentId);

                        //if parent id given then it should follow task heirarcy rule
                        if (dto.ParentId != null)
                        {
                            int checkTaskLevel = await CheckTaskHeirarchy(tasks.Id, type, ParentId, projectId);
                            if (checkTaskLevel < 0)
                            {
                                return checkTaskLevel;
                            }
                        }

                        // if follow task level rule updating it
                        tasks.ParentId = dto.ParentId;
                        tasks.TaskType = (TaskTypes)dto.type;
                    }
                }

                // checking if user want to update estimated , remaining hours or not 
                // if yes checking that remaining hours must be less than estimated hours
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

                // if user want to update description
                if (dto.Description != null)
                {
                    tasks.Description = dto.Description;
                }

                // if user want to update parent tasks only
                if (dto.type == null && dto.ParentId != null)
                {
                    // checking that tobeupdated parent is valid or not
                    var parentTask = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == dto.ParentId && t.IsActive);
                    if (parentTask == null)
                    {
                        return -12;
                    }

                    //check task level rule
                    var checkTaskLevel = await CheckTaskHeirarchy(tasks.Id, tasks.TaskType, parentTask.Id, projectId);
                    if (checkTaskLevel < 0)
                    {
                        return checkTaskLevel;
                    }

                    //if pass all creiteria updating parent
                    tasks.ParentId = parentTask.Id;
                }

                // updating the status directly cause input dto is already checking value from 0-2
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

                //if user want to update sprint of a task then to be updated sprint id must be of same project
                if (dto.SprintId != null)
                {
                    var sprint = await _dbContext.Sprints.Where(s => s.Id == dto.SprintId && s.ProjectId == tasks.ProjectId).FirstOrDefaultAsync();
                    if (sprint == null)
                    {
                        return -13;
                    }
                    tasks.SprintId = sprint.Id;
                }

                // if task assignee to be updated
                if (dto.AssignTo != null)
                {
                    // checking the to be updated assignee is valid or not
                    var assignTo = await _dbContext.Employees.Where(e => e.Id == dto.AssignTo).FirstOrDefaultAsync();
                    if (assignTo == null)
                    {
                        return -14;
                    }

                    // checking if to be updated assignee is in same project or not
                    var checkProjectMember = await _dbContext.ProjectEmployees.FirstOrDefaultAsync(p => p.ProjectID == projectId && p.EmployeeID == assignTo.Id);
                    if (checkProjectMember == null)
                    {
                        return -14;
                    }
                    tasks.AssignedToId = assignTo.Id;
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(int, List<GetTaskDto>?)> GetAllTasks(TaskPaginatedDto PDto)
        {
            try
            {
                // storing variables for pagination
                int pageNumber = PDto.pageNumber <= 0 ? 1 : PDto.pageNumber;
                int pageSize = PDto.pageSize <= 0 ? 10 : PDto.pageSize;
                int count = 0;

                // checking for valid project 
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == PDto.ProjectID);
                if (project == null)
                {
                    return (0, null);
                }

                // checking access of task
                if (JwtService.UserRole != EmployeeRole.SuperAdmin)
                {
                    bool checkMember = await CheckTeamMemberOfProject(JwtService.UserId, null, project.Id);
                    if (!checkMember)
                    {
                        return (-1, null);
                    }
                }

                // if task is accessible fetching the task of a given project Id
                var tasks = _dbContext.Taasks
                    .Include(e => e.AssignedBy)
                    .Include(e => e.AssignedTo)
                    .Where(e => e.ProjectId == PDto.ProjectID && e.IsActive == true)
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
                        Type = e.TaskType,
                        EstimatedHours = e.EstimateHours,
                        RemainingHours = e.RemainingHours
                    }).AsQueryable();

                if (tasks == null)
                {
                    return (0, null);
                }
                count = await tasks.CountAsync();

                //apply filtering according to type,status,sprint,AssignedTo,assigned,unassigned,date,parenttask
                
                tasks = ApplyFiltering(tasks, PDto, JwtService.UserId);
                
                if (tasks == null)
                {
                    return (0, null);
                }

                //apply sorting 
                if (string.IsNullOrEmpty(PDto.SortBy) == false && PDto.SortBy != "")
                {
                    tasks = SortingService.ApplySorting(tasks, PDto.SortBy, PDto.IsAscending);
                }
                count = tasks.Count();

                //apply pagination
                var skipResult = (pageNumber - 1) * pageSize;
                return (count, await tasks.Skip(skipResult).Take(pageSize).ToListAsync());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(int, List<GetTaskDto>?)> GetParentChildTask(int projectId, int taskId, bool children)
        {
            try
            {

                List<GetTaskDto>? resultTasks = new List<GetTaskDto>();

                //  checking if project id is valid or not
                var project = await _dbContext.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();
                if (project == null)
                {
                    return (-1, null);
                }

                // checking if task id is valid or not
                var tasks = await _dbContext.Taasks.Where(t => t.Id == taskId && t.ProjectId == projectId).FirstOrDefaultAsync();
                if (tasks == null)
                {
                    return (-2, null);
                }

                int count = 0;
                var type = tasks.TaskType;
                var toFindType = new TaskTypes();
                bool flag = true;

                // fetching all task first
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

                //filtering according to type 

                //  if user want children of task
                if (children && type != TaskTypes.Task && type != TaskTypes.Bugs) // want children
                {

                    if (type == TaskTypes.UserStory)
                    {
                        // if task type is userstory then children can be both normal task or bugs
                        query = query.Where(t => (t.Type == toFindType || t.Type == TaskTypes.Bugs) && t.ParentId != taskId);
                        flag = false;
                    }
                    else
                    {
                        toFindType = (TaskTypes)(Convert.ToInt32(type) + 1); // in heirarchy child is one level down 
                        query = query.Where(t => t.Type == toFindType && t.ParentId != taskId);
                        flag = false;

                    }
                }
                //if user want parent of task
                else if (children == false && type != TaskTypes.Epic)// want parent
                {


                    if (type == TaskTypes.Task || type == TaskTypes.Bugs)
                    {
                        // if type is task or bug task type bugs or normal task both have parent of userstory type
                        query = query.Where(t => t.Type == TaskTypes.UserStory && t.Id != tasks.ParentId);
                        flag = false;
                    }
                    else
                    {
                        toFindType = (TaskTypes)(Convert.ToInt32(type) - 1); // parent is one level up in heirarchy
                        query = query.Where(t => t.Type == toFindType && t.Id != tasks.ParentId);
                        flag = false;
                    }
                }
                if (flag) // exceptional case
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
        public async Task<int> AddTasks(AddTasksDto dtos)
        {
            try
            {
                int checkInput = await CheckInputDetails(dtos, JwtService.UserId); // return positive value if all input is correct
                if (checkInput < 0)
                {
                    return checkInput;
                }

                // if all parameter is passed creating a task
                var tasks = new Tasks
                {
                    Name = dtos.Name,
                    AssignedById = JwtService.UserId,
                    AssignedToId = dtos.AssignedToId,
                    Description = dtos.Description,
                    TaskType = dtos.type,
                    Status = dtos.Status,
                    CreatedBy = JwtService.UserId,
                    ParentId = dtos.ParentId,
                    ProjectId = dtos.ProjectId,
                    SprintId = dtos.SprintId,
                    EstimateHours = dtos.EstimateHours,
                    RemainingHours = dtos.EstimateHours
                };

                // adding task into database
                await _dbContext.Taasks.AddAsync(tasks);
                await _dbContext.SaveChangesAsync();

                //creating log of task creation
                var log = new Log
                {
                    TaskId = tasks.Id,
                    Message = $"{JwtService.Name} Created task Name : {tasks.Name} Type : {tasks.TaskType} Status : {tasks.Status}"
                };
                await _dbContext.Logs.AddAsync(log);
                await _dbContext.SaveChangesAsync();
                return tasks.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddLog(Tasks tasks, PreviousTaskValueDto dto)
        {
            try
            {
                if (dto.EHours != tasks.EstimateHours)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {JwtService.Name} Changed Estimated Hours from {dto.EHours} to {tasks.EstimateHours} "
                    };
                    await _dbContext.Logs.AddAsync(log);
                }
                if (dto.RHours != tasks.RemainingHours)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {JwtService.Name} Changed Remaining Hours from {dto.RHours} to {tasks.RemainingHours} "
                    };
                    await _dbContext.Logs.AddAsync(log);
                }
                if (dto.TaskType != tasks.TaskType)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {JwtService.Name} Changed Task Type from {dto.TaskType.ToString()}  to {tasks.TaskType.ToString()}"
                    };
                    await _dbContext.Logs.AddAsync(log);
                }
                var UpdatedParent = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasks.ParentId);
                if (dto.ParentId != tasks.ParentId)
                {

                    if (dto.Parent != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Changed Parent Task from {dto.Parent.Id} : {dto.Parent.Name} to {UpdatedParent.Id}" +
                            $": {UpdatedParent.Name} "
                        };
                        await _dbContext.Logs.AddAsync(log);

                    }
                    else
                    {

                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Added Parent Task  {UpdatedParent.Id}" +
                            $": {UpdatedParent.Name} "
                        };
                        await _dbContext.Logs.AddAsync(log);

                    }
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Added Child Task {tasks.Id} : {tasks.Name}" +
                            $" in {UpdatedParent.Id} " +
                                $": {UpdatedParent.Name} "
                        };
                        await _dbContext.Logs.AddAsync(log);
                    }
                }
                if (dto.Description != tasks.Description)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {JwtService.Name} Changed Description from {dto.Description}  to {tasks.Description}"
                    };
                    _dbContext.Logs.Add(log);
                }
                if (dto.TaskStatus != tasks.Status)
                {
                    var log = new Log
                    {
                        TaskId = tasks.Id,
                        Message = $" {JwtService.Name} Changed Status from {dto.TaskStatus.ToString()}  to {tasks.Status.ToString()}"
                    };
                    await _dbContext.Logs.AddAsync(log);
                }
                if (dto.AssignToId != tasks.AssignedToId)
                {
                    var updatedAssignTo = await _dbContext.Employees.Where(e => e.Id == tasks.AssignedToId).FirstOrDefaultAsync();
                    if (dto.AssignTo != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Changed AssingTo from {dto.AssignToId} : {dto.AssignTo.Name} to {updatedAssignTo.Id}" + $": {updatedAssignTo.Name} "
                        };
                        await _dbContext.Logs.AddAsync(log);
                    }
                    else
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Added AssignTo  {updatedAssignTo.Id}" +
                            $": {updatedAssignTo.Name} "
                        };
                        await _dbContext.Logs.AddAsync(log);
                    }
                }
                if (dto.SprintId != tasks.SprintId)
                {
                    var Updatedsprint = await _dbContext.Sprints.Where(s => s.Id == tasks.SprintId).FirstOrDefaultAsync();
                    if (dto.Sprint != null)
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Changed Sprint from {dto.SprintId} : {dto.Sprint.Name} to {Updatedsprint.Id}" + $": {Updatedsprint.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                    else
                    {
                        var log = new Log
                        {
                            TaskId = tasks.Id,
                            Message = $" {JwtService.Name} Added Srpint  {Updatedsprint.Id}" +
                            $": {Updatedsprint.Name} "
                        };
                        _dbContext.Logs.Add(log);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateTasks(UpdateTasksDto dto, int id)
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
                var Accessor = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == JwtService.UserId && e.IsActive);
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
                var previousTaskValue = new PreviousTaskValueDto
                {
                    EHours = tasks.EstimateHours,
                    RHours = tasks.RemainingHours,
                    Description = tasks.Description,
                    TaskType = tasks.TaskType,
                    TaskStatus = tasks.Status,
                    ParentId = tasks.ParentId,
                    Parent = tasks.ParentId != null ? await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasks.ParentId) : null,
                    SprintId = tasks.SprintId,
                    Sprint = tasks.SprintId != null ? await _dbContext.Sprints.FirstOrDefaultAsync(s => s.Id == tasks.SprintId) : null,
                    AssignToId = tasks.AssignedToId,
                    AssignTo = tasks.AssignedToId != null ? await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == tasks.AssignedToId) : null
                };

                // if valid task found checking accessibility 
                // Who can access the task : 
                // -- a user who has assigned task
                // -- or a user whom the task has been assigned
                // -- or superadmin
                // -- or project team member 
                // -- or the manager of the user whom the task has been assigned 
                // all the above have the access the update the task

                int checkAccess = await CheckTaskAccess(JwtService.UserRole, tasks, JwtService.UserId, project); // returns 1 if task is accessible
                if (checkAccess < 0)
                {
                    return checkAccess;
                }

                // if task is accessible updating task according to given details 
                int updated = await UpdateTaskDetails(tasks, dto, project.Id); // returns 1 if successfully updated 
                if (updated < 0)
                {
                    return updated;
                }

                // Adding Logs for changes
                bool logs = await AddLog(tasks, previousTaskValue);
                tasks.UpdatedBy = JwtService.UserId;
                tasks.UpdatedOn = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return tasks.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteTasks(int id)
        {
            try
            {
                var tasks = await _dbContext.Taasks.FirstOrDefaultAsync(e => e.Id == id);
                if (tasks == null || !tasks.IsActive)
                {
                    return false;
                }
                if (tasks.AssignedById != JwtService.UserId && JwtService.UserRole != EmployeeRole.SuperAdmin)
                {
                    return false;
                }
                tasks.IsActive = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        /*public IQueryable<GetTaskDto>? ApplySorting(IQueryable<GetTaskDto>? tasks, string? SortBy,
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
*/

        /*public async Task<List<GetTaskByIdDto>?> GetTaskById(int assigneToId)
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
        }*/
    }
}
