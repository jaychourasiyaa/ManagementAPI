using ManagementAPI.Contract.Dtos.LogDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoggerServices LoggerServices;
        public LogController(ILoggerServices ls)
        {
            LoggerServices = ls;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRespones<List<GetLogDto>?>>> Get(int id)
        {
            var respones = new ApiRespones<List<GetLogDto>?>();
            try
            {
                var logs = await LoggerServices.GetLogById(id);
                if(logs == null)
                {
                    respones.Message = "Task Id is invalid or No Log Found with given taskId";
                    return NotFound(respones);
                }
                respones.Message = "Logs Fetched";
                respones.Data = logs;
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
