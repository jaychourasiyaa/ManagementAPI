using Azure;
using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Identity.Client;
using System.Security.Claims;
namespace ManagementAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly dbContext _dbContext;
    private readonly ITasksServices tasksService;
    public TasksController(dbContext db, ITasksServices tasksservices)
    {
        _dbContext = db;
        tasksService = tasksservices;
    }
    [HttpGet]
 
    public async Task<ActionResult<List<GetTaskDto>?>> Get()
    {
        var respones = new ApiRespones<List<GetTaskDto>?>();
        try
        {
            EmployeeRole Role = EmployeeRole.Employee;
            var RoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
            {
                Role = (EmployeeRole)RoleEnum;
            }
            int assignedTo = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var result = await tasksService.GetAllTasks(Role, assignedTo);
            if( result == null)
            {
                respones.Message = "No Task Found";
                return NotFound(respones);
            }
            respones.Message = "Task Fetched";
            respones.Data = result;
            return Ok(respones);
        }
        catch (Exception ex)
        {
            respones.Success = false;
            respones.Message = ex.Message;
            return BadRequest(respones);
        }
        /*var tasks = tasksService.GetAllTasks();
        return Ok(tasks);*/
    }
    /*[HttpPost("SelfTaskAssign")]*/
    /*public async Task<ActionResult<ApiRespones<int>>> SelfTask (AddTasksDtos dto)
    {
        var response = new ApiRespones<int >();
        try
        {
            int assignedBy = int.Parse(User.Claims.FirstOrDefault(e => e.Type == "Id")?.Value);
            var result = tasksService.AssignSelfTask(dto, assignedBy );
            response.Message = "Task Assigned to self";
            response.Data = result;
            return Ok(response);    
        }
        catch(Exception ex)
        {
            response.Message = ex.Message;
            response.Success = false;
            return BadRequest(response);
        }
    }*/
    [HttpPost]
    public async Task<ActionResult<ApiRespones<int?>>> Add(AddTasksDtos tasksDtos)
    {
        var response = new ApiRespones<int?>();
        try
        {
            int assignedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            if(tasksDtos.AssignedToId == 0) { tasksDtos.AssignedToId = assignedBy; }
            int tskid = await tasksService.AddTasks(tasksDtos , assignedBy);
            
            if (tskid == -1)
            {
                response.Message = "Task Assigner Id is invalid";
                return NotFound(response);
            }
            if (tskid == -2)
            {
                response.Message = "Employee Id is invalid";
                return NotFound(response);
            }
            if (tskid == -3)
            {
                response.Message = "Task Assigner is not a manager of given Employee Id";
                return NotFound(response);
            }
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
    public async Task<ActionResult<ApiRespones<int?>>> Update(int status, int id, int AccessingId)
    {
        var response = new ApiRespones<int?>();
        try
        {
            int responseId = await tasksService.UpdateTasks(status, id, AccessingId);
            
            if (responseId == -1)
            {             
                response.Message = "Task Id is wrong";
                return NotFound(response);
            }
            if (responseId == -2)
            {                
                response.Message = "Task is not accessible check Id";
                return Conflict(response);
            }
            if (responseId == -3)
            {               
                response.Message = "Status is out of range";
                return Conflict(response);
            }
            response.Message = "Task Status Updated";
            response.Data = responseId;
            return Ok(response);
        }
        catch( Exception ex) 
        { 
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response); 
        }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiRespones<bool?>>> Delete(int id, int AccessingId)
    {
        var response = new ApiRespones<bool?>();
        try
        {

            bool Deleted = await tasksService.DeleteTasks(id, AccessingId);
            
            if (!Deleted)
            {
               
                response.Message = "Task Id is wrong or Task is unaccessible";
                return NotFound(response);
            }
            response.Message = "Task Deleted";
            response.Data = Deleted;
            return Ok(response);
        }
        catch ( Exception ex )
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }

    }
    [HttpGet("GetById")]
    public async Task<ActionResult<ApiRespones<List<TaskDtos>?>>> GetbyId( )
    {
        var response = new ApiRespones<List<TaskDtos>?>();
        try
        {
            var assignedTo = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var result = await tasksService.GetTaskById(assignedTo);
            if( result.Count ==0)
            {
                response.Message = "No Tasks Founed";
                return NotFound(response);
            }
            response.Message = "Tasks Fetched";
            response.Data = result;
            response.TotalEntriesCount = result.Count;  
            return Ok(response);
        }
        catch( Exception ex )
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

}
