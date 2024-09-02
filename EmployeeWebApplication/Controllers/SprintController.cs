using ManagementAPI.Contract.Dtos.SprintDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintServices SprintServices;
        public SprintController( ISprintServices sprintservices)
        {
            this.SprintServices = sprintservices;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRespones<List<GetSprintDto>?>>> Get(int id)
        {
            var respones = new ApiRespones<List<GetSprintDto>?>();
            try
            {
                var sprints = await SprintServices.GetSprintUnderProject(id);
                if (sprints == null || sprints.Count == 0)
                {
                    respones.Message = "No Sprints Found with that Project Id";
                    return NotFound(respones);
                }
                respones.Message = "All Sprints Fetched";
                respones.Data = sprints;
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Message = ex.Message;
                respones.Success = false;
                return BadRequest(respones);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiRespones<int?>>> Upsert(AddSprintDto dto , int ? toBeUpdated )
        {
            var respones = new ApiRespones<int?>();
            try
            {
                var sprintId = await SprintServices.AddSprint(dto , toBeUpdated);
                if( sprintId == -1)
                {
                    respones.Message = "Start Date should not be greater than end date";
                    return Conflict(respones);
                }
                else if( sprintId == -2 )
                {
                    respones.Message = "Project Id is invalid";
                    return NotFound(respones);  
                }
                else if( sprintId == -3)
                {
                    respones.Message = "Sprint Id is invalid";
                    return NotFound(respones);
                }
                if (toBeUpdated == null)
                {
                    respones.Message = "Sprint Added";
                }
                else 
                { 
                    respones.Message = "Sprint Updated"; 
                }
                respones.Data = sprintId;
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Message = ex.Message;
                respones.Success = false;
                return BadRequest(respones);   
            }
        }
        
    }
}
