using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ManagementAPI.Controllers
{
    [Authorize ( Roles = "SuperAdmin")]
   
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentServices departmentServices;
        public DepartmentController(IDepartmentServices departmentservices)
        {
            this.departmentServices = departmentservices;

        }

        
        [HttpGet]
        public async Task<ActionResult<ApiRespones<List<DepartmentDtos>?>>> Get()
        {
            var response = new ApiRespones<List<DepartmentDtos>?>();
            try
            {
                var hhtp = HttpContext.User.Claims;
                List<DepartmentDtos>? department = await departmentServices.GetDepartment();
                
                if (department == null)
                {
                    response.Message = "No Department Found";
                    return NotFound(response);
                }
                response.Message = "Fetched All Employee";
                response.Data = department;
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

        public async Task<ActionResult<ApiRespones<int?>>> Add(AddDepartmentDtos departmentDtos)
        {
           
           

            // Extract the user ID from the token claims
            int createdBy = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            

            var response = new ApiRespones<int?>();
            try
            {

                int departmentId = await departmentServices.AddDepartment(departmentDtos , createdBy);
            
                if (departmentId == -1)
                {
                    
                    response.Message = "Department Already Exists";
                    return Conflict(response);
                }
             
                response.Message = "Department Added";
                response.Data = departmentId;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
               
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
                bool deleted = await departmentServices.DeleteDepartment(id , deletedBy);
      
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
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<ActionResult<ApiRespones<List<EmployeeDto?>>>> GetEmployoeesUnderDepartment(int id)
        {
            var response =new ApiRespones<List<EmployeeDto?>>();
            try
            {
                List<EmployeeDto?> employees = await departmentServices.GetEmployeeDetails(id);
               
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

    }

}
