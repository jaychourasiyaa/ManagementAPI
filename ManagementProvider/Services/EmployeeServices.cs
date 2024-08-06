using ManagementAPI.Provider.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIEmployee;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ManagementAPI.Contract.Responses;
using Employee_Role;
using System.Data;
namespace ManagementAPI.Provider.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly dbContext _dbcontext;
        public EmployeeServices(dbContext db)
        {
            _dbcontext = db;
        }
        public async Task<bool> CheckDepartment(int? id)
        {
            try
            {


                var department = await _dbcontext.Department.FindAsync(id);
                if (department == null || !department.IsActive) return false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> CheckAdmin(int? id)
        {
            try
            {
                
                var employee = await _dbcontext.Employees.FindAsync(id);
                if (employee == null || !employee.IsActive) return false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignDepartmentandManager (EmployeeDto employeeDto, AddEmployeeDtos addemployeeDto)
        {
            try
            {
                // here four conditon arise 
                // 
                // --> department id and admin id both are null
                if (addemployeeDto.DepartmentId == null && addemployeeDto.AdminId == null)
                {
                    employeeDto.DepartmentId = null;
                    employeeDto.AdminId = null;
                }
                // --> department id is not null but admin id is null
                else if (addemployeeDto.AdminId == null && addemployeeDto.DepartmentId != null)
                {
                    employeeDto.DepartmentId = addemployeeDto.DepartmentId;
                    employeeDto.AdminId = null;
                }
                // --> department id and admin id both are not null 
                else if (addemployeeDto.DepartmentId != null && addemployeeDto.AdminId != null)
                {
                    // here two more case arise whether the admin has of same department or not 
                    // if admin and employee has same department 

                    if (await CheckAdminAndEmployeeDepartment(addemployeeDto.DepartmentId, addemployeeDto.AdminId))
                    {
                        employeeDto.DepartmentId = addemployeeDto.DepartmentId;
                        employeeDto.AdminId = addemployeeDto.AdminId;
                    }
                    // if admin has different department
                    else
                    {
                        employeeDto.DepartmentId = addemployeeDto.DepartmentId;
                        employeeDto.AdminId = null;
                    }
                }
                // --> department id is null but admin id is not null
                else
                {
                    employeeDto.DepartmentId = addemployeeDto.DepartmentId;
                    employeeDto.AdminId = addemployeeDto.AdminId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> CheckAdminAndEmployeeDepartment(int? departmentId, int? adminId)
        {
            try
            {
                var Admin = await _dbcontext.Employees.FindAsync(adminId);
                if (Admin.DepartmentId == departmentId) return true;

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task UpdateDepartmentandManager(Employee employee, AddEmployeeDtos addemployeeDto)
        {
            try
            {
                // there are two nullabe feild in dto so here four conditon arise 
                // 
                // --> department id and admin id both are null
                if (addemployeeDto.DepartmentId == null && addemployeeDto.AdminId == null)
                {


                    employee.DepartmentId = null;
                    employee.AdminId = null;


                }
                // --> department id is not null but admin id is null
                else if (addemployeeDto.AdminId == null && addemployeeDto.DepartmentId != null)
                {

                    employee.DepartmentId = addemployeeDto.DepartmentId;
                    employee.AdminId = null;


                }
                // --> department id and admin id both are not null 
                else if (addemployeeDto.DepartmentId != null && addemployeeDto.AdminId != null)
                {
                    // here two more case arise whether the admin has of same department or not 

                    // if admin and employee has same department 
                    /*var check = */
                    if (await CheckAdminAndEmployeeDepartment(addemployeeDto.DepartmentId, addemployeeDto.AdminId))
                    {

                        employee.DepartmentId = addemployeeDto.DepartmentId;
                        employee.AdminId = addemployeeDto.AdminId;

                    }
                    // if admin has different department
                    else
                    {

                        employee.DepartmentId = addemployeeDto.DepartmentId;
                        employee.AdminId = null;

                    }
                }
                // --> department id is null but admin id is not null
                else
                {
                    var Admin = await _dbcontext.Employees.FindAsync(addemployeeDto.AdminId);

                    employee.DepartmentId = Admin.DepartmentId;
                    employee.AdminId = addemployeeDto.AdminId;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IQueryable<EmployeeDto>? ApplyFilering(IQueryable<EmployeeDto>? employee, string filterOn,
            int managerId)
        {
            
            if (string.IsNullOrEmpty(filterOn) == false )
            {
                // filer according to name or subpart of name
                if (filterOn.Equals("AdminId", StringComparison.OrdinalIgnoreCase))
                {
                    employee = employee.Where(e => e.AdminId == managerId);
                }
          
             }
            return employee; 
        }
            public async Task<int> AddEmployee(AddEmployeeDtos addemployeeDto , int createdBy)
        {
            try
            {
                // checking for unique username
                
                var username = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Username == addemployeeDto.Username);
                
                if( username != null )
                {
                    return -5;
                }
                
                // check if role is valid or not 
                if (Convert.ToInt32(addemployeeDto.Role) > 2 || Convert.ToInt32(addemployeeDto.Role) < 0)
                    return -4;
                // check if department and admin id is 0 
                if (addemployeeDto.DepartmentId == 0) addemployeeDto.DepartmentId = null;
                if (addemployeeDto.AdminId == 0) addemployeeDto.AdminId = null;
                if (addemployeeDto.Role == EmployeeRole.SuperAdmin && addemployeeDto.AdminId != null)
                {
                    return -3;
                }
                // checking if department and admin id/manager id is valid if not returning
                if (addemployeeDto.DepartmentId != null &&  ! await CheckDepartment(addemployeeDto.DepartmentId))
                    return -2;
                if (addemployeeDto.AdminId != null  && !await CheckAdmin(addemployeeDto.AdminId))
                    return -1;
                
                EmployeeDto employeeDto = new EmployeeDto
                {
                    Name = "",
                    Salary = 0,
                    AdminId = null,
                    DepartmentId = null
                };
                employeeDto.Name = addemployeeDto.Name;
                employeeDto.Salary = addemployeeDto.Salary;
                employeeDto.Role = addemployeeDto.Role;
                employeeDto.CreatedBy = createdBy;

                /*Console.WriteLine("id is " +createdBy);*/
                await AssignDepartmentandManager(employeeDto, addemployeeDto);
                Employee employee = new Employee
                {
                    Name = employeeDto.Name,
                    Salary = employeeDto.Salary,
                    Role = employeeDto.Role,
                    DepartmentId = employeeDto.DepartmentId != null ? employeeDto.DepartmentId : null,
                    AdminId = employeeDto.AdminId != null ? employeeDto.AdminId : null,
                    Username = addemployeeDto.Username,
                    Password = addemployeeDto.Password,
                    CreatedBy = employeeDto.CreatedBy,
                    
                };
                await _dbcontext.Employees.AddAsync(employee);
                await _dbcontext.SaveChangesAsync();
                return employee.Id;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateEmployee(AddEmployeeDtos addemployeeDto, int id , int updatedBy)
        {
            try
            {
                var employee = await _dbcontext.Employees.FindAsync(id);

                // checking if the employee exists in database or not 
                if (employee == null || !employee.IsActive)
                { return -4; }
                var username = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Username == addemployeeDto.Username);
                if (username != null)
                {
                    return -5;
                }
                // check if role is valid or not 
                if (Convert.ToInt32(addemployeeDto.Role) > 2 || Convert.ToInt32(addemployeeDto.Role) < 0)
                { return -3; }

                employee.Name = addemployeeDto.Name;
                employee.Salary = addemployeeDto.Salary;
                employee.Role = addemployeeDto.Role;
                employee.UpdatedBy = updatedBy;
                employee.UpdatedOn = DateTime.Now;
                employee.Username = addemployeeDto.Username;
                employee.Password = addemployeeDto.Password;
                if (addemployeeDto.DepartmentId == 0) addemployeeDto.DepartmentId = null;
                if( addemployeeDto.AdminId ==0) addemployeeDto.AdminId = null;
                // checking if department exists  or not
                if (addemployeeDto.DepartmentId != null && !await CheckDepartment(addemployeeDto.DepartmentId))
                { return -2; }

                // checking if admin exists or not
                if (addemployeeDto.AdminId != null && !await CheckAdmin(addemployeeDto.AdminId))
                { return -1; }
                employee.UpdatedOn = DateTime.Now;
                await UpdateDepartmentandManager(employee, addemployeeDto);
                await _dbcontext.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteEmployee(int id , int deletedBy)
        {
            try
            {

                var employee = await _dbcontext.Employees
                    .Where(e => e.Id == id && e.IsActive == true)
                    .FirstOrDefaultAsync();

                if (employee == null || !employee.IsActive) return false;
                employee.UpdatedOn = DateTime.Now;
                employee.UpdatedBy = deletedBy;
                employee.IsActive = false;
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<EmployeeDto>> GetAllEmployee(EmployeeRole Role, int managerId)
        {
            try
            {
                var employee =  _dbcontext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Admin).
                    Where(e => e.IsActive == true)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Salary = e.Salary,
                        Role = e.Role,
                        DepartmentId = e.DepartmentId,
                        AdminId = e.AdminId,
                        DepartmentName = e.Department != null ? e.Department.Name : null,
                        AdminName = e.Admin != null ? e.Admin.Name : null,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedOn = e.UpdatedOn,

                    }).AsQueryable();

                if( Role == EmployeeRole.SuperAdmin) { return await employee.ToListAsync(); }
                employee = ApplyFilering( employee, "AdminId", managerId );
                return await employee.ToListAsync();
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }

        }
        public async Task<EmployeeDto > GetDetailsById(  int id )
        {
            try
            {
                var employee = await _dbcontext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Admin)
                     .Where(e => e.Id == id && e.IsActive == true)
                     .Select(e => new EmployeeDto
                     {
                         Id = e.Id,
                         Name = e.Name,
                         Salary = e.Salary,
                         Role = e.Role,
                         DepartmentId = e.DepartmentId,
                         AdminId = e.AdminId,
                         DepartmentName = e.Department != null ? e.Department.Name : null,
                         AdminName = e.Admin != null ? e.Admin.Name : null,



                     }).FirstOrDefaultAsync();
                if (employee == null)
                    return null;
                return employee;
            }
            catch ( Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<EmployeeDto>?> GetAllManagers( )
        {
            var manager = await _dbcontext.Employees.
                Where(e => e.IsActive && _dbcontext.Employees.Any(emp => emp.AdminId == e.Id)).
                Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Salary = e.Salary,  
                    Role = e.Role,
                    DepartmentId = e.DepartmentId,
                    AdminId = e.AdminId,
                    DepartmentName = e.Department.Name,
                    AdminName = e.Admin.Name,
                    CreatedBy = e.CreatedBy,
                    UpdatedBy = e.UpdatedBy,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn,

                }).Distinct().ToListAsync();
            if (manager == null)
                return null;
            return manager; 

               
        }
    } 
}
