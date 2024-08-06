using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Models;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManagementAPI.Provider.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly dbContext _dbContext;

        private readonly IConfiguration configuration;

        public AuthServices(dbContext db, IConfiguration iconfig)
        {
            _dbContext = db;

            configuration = iconfig;
        }
        public string GenerateToken( Employee employee)
        {

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, employee.Role.ToString()),
                new Claim("Name", employee.Name),
                new Claim("Id", employee.Id.ToString()),
                new Claim("Guid", Guid.NewGuid().ToString()),

            };


            var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(5),
                    signingCredentials: credential
                );

            return tokenHandler.WriteToken(token);
        }
       
        public async Task<LoginResponseDto?> LoginUser(CredentialsDto dto)
        {
        
            try
            {
                     var employee = await _dbContext.Employees
                    .Where(e => e.Username == dto.UserName & e.IsActive)
                    .FirstOrDefaultAsync();
                if (employee == null)
                {
                    return null;
                }
                if(employee.Password != dto.Password)
                {
                    return null;
                }
                string token = GenerateToken(employee);
                var responseInfo = new LoginResponseDto
                {
                    Name = employee.Name,
                    EmployeeId = employee.Id,
                    Role = employee.Role,

                    Token = token
                };
                return responseInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
