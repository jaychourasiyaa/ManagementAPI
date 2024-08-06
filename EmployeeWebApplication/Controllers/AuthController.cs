using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPIEmployee;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly dbContext _dbContext;
        private readonly IAuthServices authServices;
        public AuthController(dbContext db, IAuthServices services)
        {
            _dbContext = db;
            authServices = services;
        }
       
        
        [HttpPost]
        [Route ( "Login")]
        public async Task<ActionResult<ApiRespones<TokenEmployeeDto?>>> Login( CredentialsDto dto)
        {
            var response = new ApiRespones <LoginResponseDto?>();
            try
            {
                var result = await authServices.LoginUser(dto);
               
                if (result == null)
                {
                  
                    response.Message = "Check Credentials ( username and password) ";
                    return BadRequest(response);
                }
               
                response.Message = "Logged In";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success= false;
                response.Message= ex.Message;
                return BadRequest(response);
                
            }
            
        }
    }
}
