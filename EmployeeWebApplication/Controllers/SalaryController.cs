/*using Azure;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly dbContext _dbContext;
        private readonly ISalaryServices salaryServices;
        public SalaryController(dbContext db, ISalaryServices services)
        {
            _dbContext = db;
            salaryServices = services;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<List<DateTime>>?> Get(int id)
        {
            var response = new ApiRespones<List<DateTime>?>();
            try
            {
                var salarydates = await salaryServices.GetById(id);
               
                if (salarydates == null)
                {
                    response.Message = "No Details Found";
                    return BadRequest(response);
                }
                response.Message = "Fetched Dates of Advance Taken by Employee";
                response.Data = salarydates;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize( Roles = "SuperAdmin" )]
        public async Task<ActionResult<ApiRespones<int?>>> Add(SalaryDtos salaryDtos)
        {
            var response = new ApiRespones<int?>();
            try
            {
                var responseId = await salaryServices.TakeAdvance(salaryDtos);
               
                if (responseId == -1)
                {
                    
                    response.Message = "Employee Not found";                  
                    return NotFound(response);
                }
                if (responseId == -2)
                {

                    response.Message = "Advance Already Taken";
                    return Conflict(response);
                }
                response.Message = "Advance Taken For this Month";
                response.Data = responseId;
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
*/