using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.DepartmentDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPI.Provider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace ManagementAPI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentServices departmentServices;
        private readonly dbContext _dbContext;
        public DepartmentController(IDepartmentServices departmentservices,dbContext db)
        {
            this.departmentServices = departmentservices;
            this._dbContext = db;

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IdandNameDto>> Get(int id)
        {
            var respones = new ApiRespones<IdandNameDto>();
            try
            {
                var result = await departmentServices.GetDepartmentById(id);
                if (result == null)
                {
                    respones.Message = "Department not exist";
                    return NotFound(respones);
                }
                respones.Data = result;
                respones.Message = "Department Fetched ";
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Message = ex.Message;
                respones.Success = false;
                return BadRequest(respones);
            }
        }

        [HttpPost("GetallDepartments")]
        public async Task<ActionResult<PaginatedApiRespones<List<GetDepartmentDtos>?>>> Get(PaginatedGetDto dto)
        {
            var response = new PaginatedApiRespones<List<GetDepartmentDtos>?>();
            try
            {
               
                List<GetDepartmentDtos>? department = await departmentServices.GetDepartment(dto);

                if (department == null)
                {
                    response.Message = "No Department Found";
                    return NotFound(response);
                }
                response.Message = "Fetched All Employee";
                response.Data = department;
                response.TotalEntriesCount = await _dbContext.Department.Where( d=>d.IsActive).CountAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);

            }
        }

        [HttpGet("GetEmployeesUnderDepartment/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<List<IdandNameDto>?>>> GetEmployoeesUnderDepartment(int id)
        {
            var response = new ApiRespones<List<IdandNameDto>?>();
            try
            {
                List<IdandNameDto>? employees = await departmentServices.GetEmployeeDetails(id);

                if (employees == null)
                {

                    response.Message = "No Employees Found (Check Department Id)";
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
                return BadRequest(response);

            }

        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddDepartmentDto Dto)
        {



            // Extract the user ID from the token claims
            int createdBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);


            var response = new ApiRespones<int?>();
            try
            {

                int departmentId = await departmentServices.AddDepartment(Dto, createdBy);

                if (departmentId == -1)
                {

                    response.Message = "Department Already Exists";
                    return Conflict(response);
                }

                response.Message = "Department Added";
                response.Data = departmentId;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);

            }
        }

        [HttpPut()]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<int?>>> Update(IdandNameDto dto)
        {
            var respones = new ApiRespones<int?>();
            try
            {
                int updatedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                var result = await departmentServices.UpdateDepartment(dto, updatedBy);
                if (result == -1)
                {
                    respones.Message = "Depatment Id is invalid";
                    return NotFound(respones);
                }
                if (result == -2)
                {
                    respones.Message = "Department with given name already exists";
                    return Conflict(respones);
                }
                respones.Message = "Department Name updated";
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiRespones<bool?>>> Delete(int id)
        {
            var response = new ApiRespones<bool?>();
            int deletedBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
            try
            {
                bool deleted = await departmentServices.DeleteDepartment(id, deletedBy);

                if (!deleted)
                {

                    response.Message = "Department not found";
                    response.Data = false;
                    return NotFound(response);
                }


                response.Message = "Department Deleted";
                response.Data = deleted;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);

            }
        }

        

        /* public async Task<ActionResult<ApiRespones<List<IdandNameDto>?>>> GetDltDepartment()
        {
            var respones = new ApiRespones<List<IdandNameDto>?>();
            try
            {
                var departments = await departmentServices.GetDeletedDepartment();
                if (departments == null)
                {
                    respones.Message = "No Department Found";
                    return NotFound(respones);
                }
                respones.Message = "Fetched all deleted department";
                respones.Data = departments;
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Success = false;
                respones.Message = ex.Message;
                return BadRequest(respones);
            }
        }
        [HttpGet("GetDepartmentBy{id}")]*/
        /*[HttpPost("ReActivate")]
        public async Task<ActionResult<ApiRespones<bool>>> Reactive(int id)
        {
            var response = new ApiRespones<bool>();
            try
            {
                bool result = await departmentServices.ReactivateDepartment(id);
                if (result == false)
                {
                    response.Message = "Department Id is incorrect";
                    return NotFound(response);
                }
                response.Message = "Reactivated Department";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
*/

    }

}
