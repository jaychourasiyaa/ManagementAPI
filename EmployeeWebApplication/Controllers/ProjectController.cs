using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Models;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
        [HttpPost]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddProjectDto projectDto)
        {
            var responses = new ApiRespones<int?>();
            try
            {
                int projectId = await projectServices.AddProject(projectDto);
                
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRespones<GetProjectDetailsDto?>>> Get(int id)
        {
            var response = new ApiRespones<GetProjectDetailsDto?>();
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
        [HttpGet]
        public async Task<ActionResult<ApiRespones<List<GetProjectDetailsDto>?>>> GetAll()
        {
            var response = new ApiRespones<List<GetProjectDetailsDto>?>();
            try
            {
                var project = await projectServices.GetAllProject();
                if (project == null)
                {
                    response.Message = "No Project Found";
                    return NotFound(response);
                }
                response.Message = "Details Fetched ";
                response.Data = project;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return (BadRequest(response));
            }
        }
    }
}
