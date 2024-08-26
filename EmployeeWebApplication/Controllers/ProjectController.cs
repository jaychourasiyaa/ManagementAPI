using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Models;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPI.Provider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using System.Security.Claims;
namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly dbContext _dbContext;
        private readonly IProjectServices projectServices;
        public ProjectController(dbContext db, IProjectServices projectservices)
        {
            _dbContext = db;
            projectServices = projectservices;
        }
        [Authorize ( Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddProjectDto projectDto)
        {
            var responses = new ApiRespones<int?>();
            try
            {
                int createdBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                int projectId = await projectServices.AddProject(projectDto ,  createdBy);
                
                if (projectId == -1)
                {
                    responses.Message = "Person Making Project is not a Super Admin or does not exist";
                    return BadRequest(responses);
                }
                if (projectId == -2)
                {
                    responses.Message = "Memebers Id is invalid";
                    return BadRequest(responses);
                }
                if (projectId == -3)
                {
                    responses.Message = "Project Status is not correct";
                    return NotFound(responses);
                }
                if( projectId == -4)
                {
                    responses.Message = "Atleast two members required in members list ";
                    return BadRequest(responses);
                }
                responses.Message = "Project Created";
                responses.Data = projectId;
                return Ok(responses);
            }
            catch (Exception ex)
            {
                responses.Success = false;
                responses.Message = ex.Message;
                return (BadRequest(responses));
            }
        }
        [HttpPut]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<int?>>> Update(int projectId, AddProjectDto dto)
        {
            var respones = new ApiRespones<int?>();
            try
            {
                int updatedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                
                var result = await projectServices.UpdateProject(projectId, dto, updatedBy);
                if( result ==-1 )
                {
                    respones.Message = "Project not exist";
                    return NotFound(respones);
                }
                if( result ==-2 )
                {
                    respones.Message = "Members Id is invalid";
                    return NotFound(respones);
                }
                respones.Message = "Project Updated";
                return Ok(respones);    
            }
            catch ( Exception ex )
            {
                respones.Success = false;
                respones.Message = ex.Message;
                return BadRequest(respones);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRespones<ProjectDetailsByIdDto?>>> Get(int id)
        {
            var response = new ApiRespones<ProjectDetailsByIdDto?>();
            try
            {
                var project = await projectServices.GetById(id);
                if (project == null)
                {
                    response.Message = "No Project found";
                    return NotFound(response);
                }
                response.Message = "Project Details Fetched";
                response.Data = project;
                return Ok(response);
            }
            catch ( Exception ex )
            {
                response.Success = false;
                response.Message = ex.Message;
                return (BadRequest(response));
            }

        }
        [Authorize]
        [HttpPost("GetAllProjects")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetProjectDetailsDto>?>>> GetAll(PaginatedGetDto dto)
        {
            var response = new PaginatedApiRespones<List<GetProjectDetailsDto>?>();
            try
            {
                int employeeId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                EmployeeRole Role = EmployeeRole.Employee;
                var RoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
               
                if (Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
                {
                    Role = (EmployeeRole)RoleEnum;
                }
                (int,List<GetProjectDetailsDto>?) project = await projectServices.GetAllProject(employeeId,dto);
                if (project.Item2 == null)
                {
                    response.Message = "No Project Found";
                    return NotFound(response);
                }
                response.Message = "Details Fetched ";
                response.TotalEntriesCount = project.Item1;
                response.Data = project.Item2;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return (BadRequest(response));
            }
        }
        [HttpDelete]
        [Authorize (Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<bool>>> Delete(int employeeId , int projectId)
        {
            var respones = new ApiRespones<bool>();
            try
            {
                var result = await projectServices.DeleteMember(employeeId,projectId);
                if( result == false)
                {
                    respones.Message = "Project or employee Id is invalid ";
                    return BadRequest(respones);
                }
                respones.Data = result;
                respones.Message = "Delted Employee from Project";
                return Ok(respones);
            }
            catch( Exception ex)
            {
                respones.Success = false;
                respones.Message= ex.Message;
                return BadRequest(respones);
            }
        }
        [HttpPost("getCount")]
        public async Task<ActionResult<ApiRespones<int?>>> getCount(int status)
        {
            var response = new ApiRespones<int?>();
            try
            {
                var result = await projectServices.GetCountStatusWise(status);
                if (result == null)
                {
                    response.Message = "Invalid status given";
                    response.Data = null;
                    return NotFound(response);
                }
                response.Message = "Fetched count with given status";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Data = null;
                return BadRequest(response);
            }
        }

        [HttpGet("GetProjectTasks")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetTaskDto>?>>> GetTasks(int? projectId, int? parenttId)
        {
            var respones = new PaginatedApiRespones<List<GetTaskDto>?>();
            try
            {
                (int, List<GetTaskDto>?) tasks = await projectServices.getTasks(projectId, parenttId);
                if (tasks.Item1 == 0)
                {
                    respones.Message = "No tasks found with given values";
                    return NotFound(respones);
                }
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
    }
}
