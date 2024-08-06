using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPIEmployee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
       /* [Authorize(Roles = "SuperAdmin")]*/
        [HttpGet("GetAllEmployee")]
        public async Task<ActionResult<ApiRespones<List<EmployeeDto?>>>> Get()
        {
            var response = new ApiRespones<List<EmployeeDto?>>();
            try
            {
                EmployeeRole Role = EmployeeRole.Employee;
                var RoleClaim = User.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Role)?.Value;
                int managerId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                if( Enum.TryParse(typeof(EmployeeRole), RoleClaim, true, out var RoleEnum))
                {
                    Role = (EmployeeRole)RoleEnum;
                }
                List<EmployeeDto?> employees = await employeeServices.GetAllEmployee(Role, managerId);

                if (employees == null)
                {

                    response.Message = "No Employees Found";
                    return NotFound(response);
                }
                response.Message = "Fetched All Employees";
                response.Data = employees;
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
                if (employeeId == -5)
                {
                    response.Message = "username already exists";
                    return Conflict(response);
                }

                if (employeeId == -1)
                {

                    response.Message = "Admin/Manager Id does not exist";
                    return NotFound(response);
                };
                if (employeeId == -2)
                {

                    response.Message = "Department Id does not exist";
                    return NotFound(response);
                };
                if( employeeId == -3)
                {
                    response.Message = "SuperAdmin cannot have manager";
                    return BadRequest(response);
                }
                if (employeeId == -4)
                {

                    response.Message = "Employee Role should be correct";
                    return NotFound(response);
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
        [HttpPut("UpdateBy{id}")]
        public async Task<ActionResult<ApiRespones<int?>>> Update(int id, AddEmployeeDtos empDto)
        {
            var response = new ApiRespones<int?>();
            var updatedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            try
            {
                int IdtoBeUpdated = await employeeServices.UpdateEmployee(empDto, id, updatedBy);
                if (IdtoBeUpdated == -5)
                {
                    response.Message = "username already exists";
                    return Conflict(response);
                }
                if (IdtoBeUpdated == -4)
                {
                    response.Message = "Employee does not exist";
                    return NotFound(response);
                };

                if (IdtoBeUpdated == -3)
                {
                    response.Message = "Employee Role should be correct";
                    return NotFound(response);
                };

                if (IdtoBeUpdated == -2)
                {
                    response.Message = "Department Id does not exist";
                    return NotFound(response);
                };

                if (IdtoBeUpdated == -1)
                {
                    response.Message = "AdminId not exist ";
                    return NotFound(response);
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
        [HttpDelete("Delete{id}")]
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
        [HttpGet("GetBy{id}")]
        public async Task<ActionResult<ApiRespones<EmployeeDto?>>> GetById(int id)
        {
            var response = new ApiRespones<EmployeeDto?>();
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
        public async Task<ActionResult<ApiRespones<List<EmployeeDto>?>>> GetManagers()
        {
            var response = new ApiRespones<List<EmployeeDto>?>();
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
                response.TotalEntriesCount = result.Count();
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
