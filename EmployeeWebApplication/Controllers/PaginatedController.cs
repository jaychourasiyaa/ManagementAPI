using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Reflection.Metadata.Ecma335;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaginatedController : ControllerBase
    {
        private readonly IPaginatedService paginatedServices;

        public PaginatedController(IPaginatedService paginatedservices)
        {
            paginatedServices = paginatedservices;
        }



        
         
        //Getting details of all Employees
        /*[HttpPost ( "GetEmployees")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetEmployeeDto>?>>> Get([FromBody] PaginatedGetDto dto)
        {
            var response = new PaginatedApiRespones<List<GetEmployeeDto?>>();
            try
            {
                List<GetEmployeeDto?> employees = await paginatedServices.GetAllEmployee(dto);

                if (employees == null)
                {

                    response.Message = "No Employees Found";
                    return NotFound(response);
                }
                response.Message = "Fetched All Employees";
                response.Data = employees;
                response.TotalEntriesCount = employees.Count();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(ex);

            }
        }
        [HttpPost ("GetDepartment")]
        public async Task<ActionResult<PaginatedApiRespones<List<DepartmentDtos>?>>> GetDepartment(PaginatedGetDto dto)
        {
            var response = new PaginatedApiRespones<List<DepartmentDtos>?>();
            List<DepartmentDtos> ? result = await paginatedServices.GetAllDepartment(dto);
            try
            {
                if (response == null)
                {
                    response.Success = false;
                    response.Message = "No Department found";
                    return NotFound(response);
                }
                response.Data = result;
                response.TotalEntriesCount = result.Count();
                return Ok(response) ;
            }
            catch ( Exception ex )
            {
               response.Success= false;
                response.Message = ex.Message; 
                return BadRequest( response );
            }
        }*/
        [HttpPost("GetManagers")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetEmployeeDto>?>>> GetManagers(PaginatedGetDto dto)
        {
            var respones = new PaginatedApiRespones<List<GetEmployeeDto>?>();
            try
            {
                var result = await paginatedServices.GetAllManagers(dto);
                if( result == null )
                {
                    respones.Message = "No Managers Found";
                    return NotFound(respones);
                }
                respones.Data = result;
                respones.TotalEntriesCount =  result.Count();
                return Ok(respones);
            }
            catch ( Exception ex )
            {
                respones.Success= false;
                respones.Message = ex.Message;
                return BadRequest( respones );
            }
        }

    }
}
