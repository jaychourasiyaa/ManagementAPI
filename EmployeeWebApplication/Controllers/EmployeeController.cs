using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPIEmployee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
namespace ManagementAPI.Contract



{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly dbContext _dbcontext;
        private readonly IEmployeeServices employeeServices;
        public EmployeesController(dbContext db, IEmployeeServices eServices)
        {
            _dbcontext = db;
            employeeServices = eServices;
        }

        //Getting details of all Employees
        [Authorize]
        [HttpPost("GetAllEmployee")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetEmployeeDto?>>>> Get(PaginatedGetDto PDto)
        {
            var response = new PaginatedApiRespones<List<GetEmployeeDto?>>();
            try
            {
                EmployeeRole Role = EmployeeRole.Employee;
                var RoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                int managerId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                if (Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
                {
                    Role = (EmployeeRole)RoleEnum;
                }
               
                (int,List<GetEmployeeDto?>) employees = await employeeServices.GetAllEmployee(Role, managerId, PDto);

                if (employees.Item2== null)
                {

                    response.Message = "No Employees Found";
                    return NotFound(response);
                }
                response.Message = "Fetched All Employees";
                response.Data = employees.Item2;
                response.TotalEntriesCount = employees.Item1;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(ex);

            }
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("AddEmployee")]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddEmployeeDtos empDto) 
        {
            var response = new ApiRespones<int?>();
            try
            {

                var createdBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                int employeeId = await employeeServices.AddEmployee(empDto, createdBy);
                
                if (employeeId == -1)
                {

                    response.Message = "Admin/Manager Id does not exist";
                    return NotFound(response);
                }
                else if (employeeId == -2)
                {

                    response.Message = "Department Id does not exist";
                    return NotFound(response);
                }
                else if (employeeId == -3)
                {
                    response.Message = "SuperAdmin cannot have manager";
                    return BadRequest(response);
                }
                else if (employeeId == -4)
                {

                    response.Message = "Employee Role should be correct";
                    return NotFound(response);
                }
                else if (employeeId == -5)
                {
                    response.Message = "username already exists";
                    return Conflict(response);
                }


                response.Message = "Added Employee";
                response.Data = employeeId;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("UpdateBy/{id}")]
        public async Task<ActionResult<ApiRespones<int?>>> Update(int id, AddEmployeeDtos empDto)
        {
            var response = new ApiRespones<int?>();
            var updatedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            try
            {
                int IdtoBeUpdated = await employeeServices.UpdateEmployee(empDto, id, updatedBy);

                 if (IdtoBeUpdated == -1)
                {

                    response.Message = "Admin/Manager Id does not exist";
                    return NotFound(response);
                }
                else if (IdtoBeUpdated == -2)
                {

                    response.Message = "Department Id does not exist";
                    return NotFound(response);
                }
                else if (IdtoBeUpdated == -3)
                {
                    response.Message = "SuperAdmin cannot have manager";
                    return BadRequest(response);
                }
                else if (IdtoBeUpdated == -4)
                {

                    response.Message = "Employee Role should be correct";
                    return NotFound(response);
                }
                else if (IdtoBeUpdated == -5)
                {
                    response.Message = "username already exists";
                    return Conflict(response);
                }
                else if( IdtoBeUpdated == -6)
                {
                    response.Message = "Employee Not Found";
                    return NotFound(response) ;
                }
                response.Message = "Employee Updated";
                response.Data = IdtoBeUpdated;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }


        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("DeleteBy/{id}")]
        public async Task<ActionResult<ApiRespones<bool?>>> Delete(int id)
        {
            var response = new ApiRespones<bool?>();
            int deletedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            try
            {
                bool Deleted = await employeeServices.DeleteEmployee(id, deletedBy);

                if (!Deleted)
                {
                    response.Message = "Employee Not found";
                    response.Data = false;
                    return NotFound(response);
                }

                response.Message = "Employee Deleted";
                response.Data = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }


        }
        [HttpGet("GetBy/{id}")]
        public async Task<ActionResult<ApiRespones<GetByIdDto?>>> GetById(int id)
        {
            var response = new ApiRespones<GetByIdDto?>();
            try
            {
                var employee = await employeeServices.GetDetailsById(id);

                if (employee == null)
                {
                    response.Message = "Employee Details not found";
                    return NotFound(response);
                }
                response.Message = "Employees Details Fetched";
                response.Data = employee;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllManagers")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<List<GetEmployeeDto>?>>> GetManagers()
        {
            var response = new ApiRespones<List<GetEmployeeDto>?>();
            try
            {

                var result = await employeeServices.GetAllManagers();
                if (result == null)
                {
                    response.Message = "No Managers Found";
                    return NotFound(response);
                }
                response.Message = "All Managers Fetched";
                response.Data = result;
             
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpGet("GetDeletedEmployees")]
        public async Task<ActionResult<ApiRespones<List<IdandNameDto>>>> GetDeleted()
        {
            var respones = new ApiRespones<List<IdandNameDto>>();
            try

            {
                var result = await employeeServices.GetDeletedEmployee();
                if (result == null)
                {
                    respones.Message = "No Deleted Employees Found";
                    return NotFound(respones);
                }
                respones.Message = "Deleted Employees Fetched";
                respones.Data = result;
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Success = false;
                respones.Message = ex.Message;
                return BadRequest(respones);
            }
        }
        /*[HttpPost("ReActivate")]
        public  async Task<ActionResult<ApiRespones<bool>>> Reactive(int id)
        {
            var response = new ApiRespones<bool>();
            try
            {
                bool result = await employeeServices.ReactivateEmployee(id);
                if (result == false)
                {
                    response.Message = "Employee Id is incorrect";
                    return NotFound(response);
                }
                response.Message = "Reactivated Employee";
                return Ok(response);
            }
            catch( Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }*/
        [HttpPost("getCount")]
        public async Task<ActionResult<ApiRespones<int?>>> getCount( int role)
        {
            var response = new ApiRespones<int?>();
            try
            {
                var result = await employeeServices.GetCountRoleWise(role);
                if( result == null)
                {
                    response.Message = "Invalid Role given";
                    response.Data = null;
                    return NotFound(response);
                }
                response.Message = "Fetched count with given role";
                response.Data = result;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.Message=ex.Message;
                response.Data = null;
                return BadRequest(response);
            }
        }
        
    }
}
