using Azure;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles ="SuperAdmin")]
    public class AttendenceController : ControllerBase
    {

        private readonly dbContext _dbContext;
        private readonly IAttendenceServices attendenceServices;
        public AttendenceController(dbContext db, IAttendenceServices aservices)
        {
            _dbContext = db;
            attendenceServices = aservices;
        }

        [HttpPost]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddAttendenceDtos adtos)
        {
            var response = new ApiRespones<int?>();
            try
            {
                int responseId = await attendenceServices.AttendenceAdd(adtos);
                if (responseId == -1)
                {
                    response.Message = "Employee does not exist";
                    return NotFound(response);
                }
                response.Message = "Attendenece Added";
                response.Data = responseId;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<List<GetAttendenceDto?>>> Get(int id)
        {
            var response = new ApiRespones<List<GetAttendenceDto?>>();
            try
            {
                var attendence = await attendenceServices.getAttendenceList(id);
                if (attendence == null)
                {
                    response.Message = "Attendence Not found Check employee Id";
                    return NotFound(response);
                }
                response.Message = "Fetched Attendence";
                response.Data = attendence;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
               
            }
            
        }
    }
}
