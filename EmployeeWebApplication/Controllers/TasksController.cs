﻿using Azure;
using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPI.Provider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using System.Security.Claims;
namespace ManagementAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
   
    private readonly ITasksServices tasksService;
    private readonly IJwtService jwtService;
    public TasksController( ITasksServices tasksservices ,IJwtService jwtservice)
    {
        tasksService = tasksservices;
        jwtService = jwtservice;
    }
    [HttpPost("GetAllTasks")]
    public async Task<ActionResult<PaginatedApiRespones<List<GetTaskDto>?>>> Get(TaskPaginatedDto dto)
    {
        var respones = new PaginatedApiRespones<List<GetTaskDto>?>();
        try
        {
            //fetching role and userid from claim using jwt service 
            EmployeeRole Role = jwtService.UserRole;
            int assignedTo = jwtService.UserId;

            //calling service that return all tasks rolewise with filtering , sorting , pagination
            (int, List<GetTaskDto>?) result = await tasksService.GetAllTasks(Role, assignedTo, dto);

            // if not task found 
            if (result.Item2 == null)
            {
                respones.Message = "No Task Found";
                return NotFound(respones);
            }

            // returning fetched task
            respones.Message = "Task Fetched";
            respones.TotalEntriesCount = result.Item1;
            respones.Data = result.Item2;
            return Ok(respones);
        }
        catch (Exception ex)
        {
            respones.Success = false;
            respones.Message = ex.Message;
            return BadRequest(respones);
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiRespones<int?>>> Add(AddTasksDtos dto)
    {
        var response = new ApiRespones<int?>();
        try
        {
            // fetching logged in employee id from claims
            int assignedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            
            //calling service that can add task 
            int tskid = await tasksService.AddTasks(dto, assignedBy);
            if (tskid == -1)
            {
                response.Message = "Given Task Type must required a parent id";
                return Conflict(response);
            }
            else if (tskid == -2)
            {
                response.Message = "Task Assigner Id is invalid";
                return NotFound(response);
            }
            else if (tskid == -3)
            {
                response.Message = "Project Id is invalid";
                return NotFound(response);
            }
            else if (tskid == -4)
            {
                response.Message = "Employee Id is invalid";
                return NotFound(response);
            }
            else if (tskid == -5)
            {
                response.Message = "Employee is not the member of given projectId";
                return NotFound(response);
            }
            else if (tskid == -6)
            {
                response.Message = "Task Assigner is neither a Superadmin nor a manager nor team member of given Employee Id";
                return NotFound(response);
            }
            else if (tskid == -7)
            {
                response.Message = "Feature Type should contain parent id only of Epic Type" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -8)
            {
                response.Message = "User Story Type should contain parent id only of Feature Type" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -9)
            {
                response.Message = "Task Type should contain parent id only of User Story Type " +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -10)
            {
                response.Message = "Bug Type should contain parent id only of User Story Type +" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -11)
            {
                response.Message = "Sprint Id is invalid or sprint id is not of same project id";
                return NotFound(response);
            }

            // adding task and sending respones accordingly
            response.Message = "Task Added";
            response.Data = tskid;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiRespones<int?>>> Update(UpdateTasksDto dto, int id)
    {
        var response = new ApiRespones<int?>();
        try
        {
            // fetching logged in employee id and role form claims
            int AccessingId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            EmployeeRole Role = EmployeeRole.Employee;
            var RoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
            {
                Role = (EmployeeRole)RoleEnum;
            }
            
            //calling service that can update task
            int tskid = await tasksService.UpdateTasks(dto, id, AccessingId, Role);
            if (tskid == -1)
            {
                response.Message = "Task Type Epic cannot have parent";
                return Conflict(response);
            }
            else if (tskid == -2)
            {
                response.Message = "Given Task Type must required a parent id";
                return Conflict(response);
            }
            else if (tskid == -3)
            {
                response.Message = "Accessing Id is invalid";
                return NotFound(response);
            }
            else if (tskid == -4)
            {
                response.Message = "Project id is invalid";
                return NotFound(response);
            }
            else if (tskid == -5)
            {
                response.Message = "Task Id is invalid with given taskId and projectId";
                return NotFound(response);
            }
            else if (tskid == -6)
            {
                response.Message = "Task is unaccessible";
                return Conflict(response);
            }
            else if (tskid == -7)
            {
                response.Message = "Feature Type should contain parent id only of Epic Type" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -8)
            {
                response.Message = "User Story Type should contain parent id only of Feature Type" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -9)
            {
                response.Message = "Task Type should contain parent id only of User Story Type " +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -10)
            {
                response.Message = "Bug Type should contain parent id only of User Story Type +" +
                    "and of same project Id";
                return Conflict(response);
            }
            else if (tskid == -11)
            {
                response.Message = "Estimate Hours must be greater than remaining hours";
                return Conflict(response);
            }
            else if (tskid == -12)
            {
                response.Message = "Parent Id is invalid";
                return NotFound(response);
            }
            else if ( tskid == -13)
            {
                response.Message = "Sprint Id is invalid or not found aligned with given project Id";
                return NotFound(response);
            }
            else if( tskid == -14)
            {
                response.Message = "AssignToId is invalid or not aligned with project member Id ";
                return NotFound(response);
            }
            else if( tskid == -99)
            {
                response.Message = "Cannot Make a Task Parent to itself";
                return Conflict(response);
            }

            // task got updated sending respones accordingly
            response.Message = "Task Status Updated";
            response.Data = tskid;
            return Ok(response);

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiRespones<bool?>>> Delete(int id)
    {
        var response = new ApiRespones<bool?>();
        try
        {
            // fetching logged in employee id and role
            int AccessingId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            EmployeeRole Role = new EmployeeRole();
            var RoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
            {
                Role = (EmployeeRole)RoleEnum;
            }

            // calling service to delete the task
            bool Deleted = await tasksService.DeleteTasks(id, AccessingId, Role);

            if (!Deleted)
            {
                response.Message = "Task Id is wrong or Task is unaccessible to delete";
                return NotFound(response);
            }
            response.Message = "Task Deleted";
            response.Data = Deleted;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }

    }

    [HttpPost("GetParentChildren")]
    public async Task<ActionResult<PaginatedApiRespones<List<GetTaskDto>?>>> GetTasks(GetParentChildrenTask dto)
    {
        var respones = new PaginatedApiRespones<List<GetTaskDto>?>();
        try
        {
            (int, List<GetTaskDto>?) tasks = await tasksService.getTasks(dto.ProjectId, dto.TaskId ,dto.WantChild);
            if (tasks.Item1 == 0)
            {
                respones.Message = "No tasks found with given values";
                return NotFound(respones);
            }
            else if( tasks.Item1 == -1)
            {
                respones.Message = "ProjectId is invalid not found with given project Id";
                return NotFound(respones);
            }
            else if (tasks.Item1 == -2)
            {
                respones.Message = "TaskId is invalid not found with given task and project Id";
                return NotFound(respones);
            }
            respones.Message = "Task Fetched";
            respones.Data = tasks.Item2;
            respones.TotalEntriesCount = tasks.Item1;
            return Ok(respones);
        }
        catch (Exception ex)
        {
            respones.Success = false;
            respones.Message = ex.Message;
            return BadRequest(respones);
        }
    }
    /*[HttpGet("GetById")]
    public async Task<ActionResult<ApiRespones<List<GetTaskByIdDto>?>>> GetById()
    {
        var response = new ApiRespones<List<GetTaskByIdDto>?>();
        try
        {
            var assignedTo = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var result = await tasksService.GetTaskById(assignedTo);
            if (result.Count == 0)
            {
                response.Message = "No Tasks Founed";
                return NotFound(response);
            }
            response.Message = "Tasks Fetched";
            response.Data = result;

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }*/
    

}
