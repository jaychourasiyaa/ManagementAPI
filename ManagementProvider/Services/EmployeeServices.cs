using ManagementAPI.Provider.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIEmployee;
using ManagementAPI.Contract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ManagementAPI.Contract.Responses;
using Employee_Role;
using System.Data;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Dtos;
namespace ManagementAPI.Provider.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly dbContext _dbcontext;
        public EmployeeServices(dbContext db)
        {
            _dbcontext = db;
        }
        public IQueryable<GetEmployeeDto>? ApplyFilering(IQueryable<GetEmployeeDto>? employee, string? filterOn,
            string? filterQuery ,DateTime ? startDate, DateTime ? endDate)
        {
            if( startDate != null && endDate != null )
            {
                employee = employee.Where( e=> e.CreatedOn  >= startDate && e.CreatedOn <= endDate );
            }
            if (string.IsNullOrEmpty(filterQuery) == false)
            {
                // filer according to name or subpart of name

                if (int.TryParse(filterQuery, out int result))
                {
                    employee = employee.Where(e => e.Salary == result);
                }
                else if (filterOn.Equals("", StringComparison.OrdinalIgnoreCase))
                {
                    employee = employee.Where(e => e.Name.Contains(filterQuery)
                    || e.DepartmentName.Contains(filterQuery)
                    || e.AdminName.Contains(filterQuery));
                }
                // filter according to department or subpart of department name
                /*else if (filterOn.Equals("DepartmentName", StringComparison.OrdinalIgnoreCase))
                {*/
                /* employee = employee.Where(e => e.DepartmentName.Contains(filterQuery));*/
                /*}*/
                /* else if (filterOn.Equals("AdminName", StringComparison.OrdinalIgnoreCase))
                 {*/
                /* employee = employee.Where(e => e.AdminName.Contains(filterQuery));*/
                /*}
                else if (filterOn.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
                {*/
                /*employee = employee.Where(e => e.CreatedBy.Contains(filterQuery));*/
                /*}*/
                // filer according to role
                else if (filterOn.Equals("Role", StringComparison.OrdinalIgnoreCase))
                {
                    // Checking value of filterQuery is number or not
                    if (int.TryParse(filterQuery, out var roleId))
                    {
                        // if number then check for valid number from 0-2
                        if (Enum.IsDefined(typeof(EmployeeRole), roleId))
                        {
                            var role = (EmployeeRole)roleId;
                            employee = employee.Where(e => e.Role == role);
                        }
                        else
                        {
                            throw new Exception("Invalid role ID specified.");
                        }
                    }
                    else
                    {
                        // Normalize filterQuery to match enum names case insensitively
                        var normalizedFilterQuery = filterQuery.Trim().ToLowerInvariant();

                        var matchingRole = Enum.GetValues(typeof(EmployeeRole))
                            .Cast<EmployeeRole>()
                            .FirstOrDefault(role => role.ToString().ToLowerInvariant() == normalizedFilterQuery);

                        /*  if (matchingRole != default(EmployeeRole))
                          {*/
                        employee = employee.Where(e => e.Role == matchingRole);
                        /*}
                        else
                        {
                            throw new Exception("Invalid role name specified.");
                        }*/
                    }
                }
            }
            return employee;
        }

        public IQueryable<GetEmployeeDto>? ApplySorting(IQueryable<GetEmployeeDto>? employee, string? SortBy,
            bool IsAscending)
        {
            if (string.IsNullOrEmpty(SortBy) == false)
            {
                if (SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {

                    employee = IsAscending ? employee.OrderBy(e => e.Name) : employee.OrderByDescending(e => e.Name);

                }
                if (SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.CreatedOn) :
                    employee.OrderByDescending(e => e.CreatedOn);

                }
                /*if (SortBy.Equals("UpdatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.UpdatedOn) :
                        employee.OrderByDescending(e => e.UpdatedOn);
                }*/
                if (SortBy.Equals("Salary", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.Salary) :
                        employee.OrderByDescending(e => e.Salary);
                }
                if (SortBy.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.CreatedBy) :
                        employee.OrderByDescending(e => e.CreatedBy);
                }
                /*if (SortBy.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.UpdatedBy) :
                        employee.OrderByDescending(e => e.UpdatedBy);
                }*/
                if (SortBy.Equals("Role", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.Role) :
                        employee.OrderByDescending(e => e.Role);
                }
                if (SortBy.Equals("DepartmentName", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.DepartmentName) :
                          employee.OrderByDescending(e => e.DepartmentName);
                }
                if (SortBy.Equals("AdminName", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.AdminName) :
                          employee.OrderByDescending(e => e.AdminName);
                }
                /*if (SortBy.Equals("DepartmentId", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.DepartmentId) :
                          employee.OrderByDescending(e => e.DepartmentId);
                }
                if (SortBy.Equals("AdminId", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.AdminId) :
                          employee.OrderByDescending(e => e.AdminId);
                }*/
                /*if (SortBy.Equals("IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.IsActive) :
                          employee.OrderByDescending(e => e.IsActive);
                }*/
            }
            return employee;
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
        public async Task AssignDepartmentandManager(Employee employee, AddEmployeeDtos addemployeeDto)
        {
            try
            {
                // here four conditon arise 
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
                    employee.DepartmentId = addemployeeDto.DepartmentId;
                    employee.AdminId = addemployeeDto.AdminId;
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
        public async Task<int> CheckInputDetails(AddEmployeeDtos addemployeeDto, int? employeeId)
        {





            // checking for unique username
            bool username = false;

            // When adding employee we dont need to check username is already binded with Id or not
            if (employeeId == null)
            {
                var employee = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Username == addemployeeDto.Username);
                if (employee != null)
                {
                    username = true;
                }
            }
            // but when updating employee we need to check that username is already binded with Id
            else
            {
                var employee = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Username == addemployeeDto.Username && e.Id != employeeId);
                if (employee != null)
                {
                    username = true;
                }
            }
            //if username already exists
            if (username)
            {
                return -5;
            }

            // check if role is valid or not 
            int role = Convert.ToInt32(addemployeeDto.Role);
            if (role > 2 || role < 0)
            {
                return -4;
            }

            // check if departmentId is default as 0 if yes assinging it null
            if (addemployeeDto.DepartmentId == 0)

            {
                addemployeeDto.DepartmentId = null;
            }


            // check if adminId is default as 0 if yes assinging it null
            if (addemployeeDto.AdminId == 0)
            {
                addemployeeDto.AdminId = null;
            }

            // superadmin cannot have manager
            if (role == 2 && addemployeeDto.AdminId != null)
            {
                return -3;
            }

            // checking if department and admin id/manager id is valid if not returning
            if (addemployeeDto.DepartmentId != null && !await CheckDepartment(addemployeeDto.DepartmentId))
            {
                return -2;
            }

            if (addemployeeDto.AdminId != null && !await CheckAdmin(addemployeeDto.AdminId))
            {
                return -1;
            }
            return 1;
        }
        public async Task<int> AddEmployee(AddEmployeeDtos addemployeeDto, int createdBy)
        {
            try
            {
                //check input details 
                int checkInputDetails = await CheckInputDetails(addemployeeDto, null);

                // if input details is incorrect
                if (checkInputDetails < 0)
                    return checkInputDetails;


                Employee employee = new Employee
                {
                    Name = addemployeeDto.Name,
                    Salary = addemployeeDto.Salary,
                    Role = addemployeeDto.Role,
                    Username = addemployeeDto.Username,
                    Password = addemployeeDto.Password,
                    CreatedBy = createdBy
                };

                //checking if Manager Id given if of role SuperAdmin
                var SuperAdmin = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == addemployeeDto.AdminId && e.Role == EmployeeRole.SuperAdmin);

                // if Role is SuperAdmin no need to check manager
                if (SuperAdmin != null)
                {
                    employee.AdminId = SuperAdmin.Id;
                    employee.DepartmentId = addemployeeDto.DepartmentId;
                }
                else
                {
                    // assigning managerId and departmentId according to conditon
                    await AssignDepartmentandManager(employee, addemployeeDto);
                }

                await _dbcontext.Employees.AddAsync(employee);
                await _dbcontext.SaveChangesAsync();
                return employee.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateEmployee(AddEmployeeDtos addemployeeDto, int id, int updatedBy)
        {
            try
            {
                var employee = await _dbcontext.Employees.FindAsync(id);

                // checking if the employee exists in database or not 
                if (employee == null || !employee.IsActive)
                {
                    return -6;
                }

                //checking input details
                int checkInputDetails = await CheckInputDetails(addemployeeDto, id);

                //if input details is incorrect returing 
                if (checkInputDetails < 0)
                {
                    return checkInputDetails;
                }

                employee.Name = addemployeeDto.Name;
                employee.Salary = addemployeeDto.Salary;
                employee.Role = addemployeeDto.Role;
                employee.UpdatedBy = updatedBy;
                employee.UpdatedOn = DateTime.Now;
                employee.Username = addemployeeDto.Username;
                employee.Password = addemployeeDto.Password;
                employee.UpdatedOn = DateTime.Now;

                //checking if Manager Id given if of role SuperAdmin
                var SuperAdmin = await _dbcontext.Employees
                       .FirstOrDefaultAsync(e => e.Id == addemployeeDto.AdminId && e.Role == EmployeeRole.SuperAdmin);

                // if Role is SuperAdmin no need to check Manager
                if (SuperAdmin != null)
                {
                    employee.DepartmentId = addemployeeDto.DepartmentId;
                    employee.AdminId = SuperAdmin.Id;
                }
                else
                {
                    // condition = A person can only become manager of employee if both 
                    //             employee and manager has same department 

                    // assigning managerId and departmentId according to above conditon
                    await AssignDepartmentandManager(employee, addemployeeDto);
                }
                await _dbcontext.SaveChangesAsync();
                var updatedemployee = employee;
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteEmployee(int id, int deletedBy)
        {
            // deleting employee making it isactive status to false
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
        public async Task<(int, List<GetEmployeeDto>)> GetAllEmployee(EmployeeRole Role, int managerId, PaginatedGetDto PDto)
        {
            try
            {
                string? filterOn = "";
                if (PDto.filterOn.Equals("Role", StringComparison.OrdinalIgnoreCase))

                {
                    filterOn = "Role";
                }
                string? filterQuery = PDto.filterQuery;
                string? additionalSearch = PDto.additionalSearch;
                string? SortBy = PDto.SortBy;
                bool IsAscending = PDto.IsAscending;
                int pageNumber = PDto.pageNumber <= 0 ? 1 : PDto.pageNumber;
                int pageSize = PDto.pageSize <= 0 ? 10 : PDto.pageSize;
                DateTime ? startDate = PDto.startDate;
                DateTime? endDate = PDto.endDate;

                // fetching details of all employee present in database
                var employee = _dbcontext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Admin).
                    Where(e => e.IsActive == true)
                    .Select(e => new GetEmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Salary = e.Salary,
                        Role = e.Role,
                        AdminId = e.AdminId,
                        DepartmentName = e.Department != null ? e.Department.Name : null,
                        DepartmentId = e.DepartmentId,
                        AdminName = e.Admin != null ? e.Admin.Name : null,
                        CreatedBy = e.Creator.Name,
                       
                        
                    }).AsQueryable();
                int count = await employee.CountAsync();


                // if employee role is  superadmin showing him all employee
                // else applying role based data filtering
                if (Role != EmployeeRole.SuperAdmin)
                {
                    employee = employee.Where(e => e.AdminId == managerId);
                    count = await employee.CountAsync();
                }

                // result wihout sorting filtering and pagination
                if (PDto.filterOn == "" && PDto.filterQuery == ""
                    && PDto.SortBy == "" && PDto.IsAscending == true
                    && PDto.pageNumber == -1 && PDto.pageSize == -1)
                {
                    return (count, await employee.ToListAsync());
                }

                // Filtering
                employee = ApplyFilering(employee, filterOn, filterQuery,startDate,endDate);
                //

                //Sorting 
                employee = ApplySorting(employee, SortBy, IsAscending);
                //

                //Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                //

                // only required when we are applying searching on role based data
                if (additionalSearch != "")
                {
                    if (int.TryParse(additionalSearch, out int result))
                    {
                        employee = employee.Where(e => e.Salary == result);
                    }
                    else
                    {
                        employee = employee.Where(e => e.Name.Contains(additionalSearch)
                        || e.DepartmentName.Contains(additionalSearch)
                        || e.AdminName.Contains(additionalSearch));
                    }
                }
                return (count, await employee.Skip(skipResults).Take(pageSize).ToListAsync());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<GetByIdDto> GetDetailsById(int id)
        {
            try
            {
                // getting employee details by id
                var employee = await _dbcontext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Admin)
                     .Where(e => e.Id == id && e.IsActive == true)
                     .Select(e => new GetByIdDto
                     {
                         userName = e.Username,
                         password = e.Password,
                         Name = e.Name,
                         Salary = e.Salary,
                         Role = e.Role,
                         DepartmentName = e.Department.Name,
                         AdminName = e.Admin.Name,
                         AdminId = e.AdminId,
                         DepartmentId = e.DepartmentId
                     }).FirstOrDefaultAsync();

                // if not found returing null
                if (employee == null)
                {
                    return null;
                }
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<GetEmployeeDto>?> GetAllManagers()
        {
            try
            {
                // getting all managers from the  database
                var manager = await _dbcontext.Employees.
                    Where(e => e.IsActive && _dbcontext.Employees.Any(emp => emp.AdminId == e.Id)).
                    Select(e => new GetEmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Salary = e.Salary,
                        DepartmentName = e.Department.Name,
                        AdminName = e.Admin.Name,
                        Role = e.Role,
                        CreatedBy = e.Creator.Name,
                        CreatedOn = e.CreatedOn,
                        DepartmentId = e.DepartmentId,
                        AdminId = e.AdminId
                    }).Distinct().ToListAsync();

                // if not found returing null
                if (manager == null)
                    return null;
                return manager;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<IdandNameDto>> GetDeletedEmployee()
        {
            var employees = await _dbcontext.Employees
                .Where(e => e.IsActive == false)
                .Select(e => new IdandNameDto
                {
                    Id = e.Id,
                    Name = e.Name,


                }).ToListAsync();
            if (employees == null) return null;
            return employees;
        }
        public async Task<bool> ReactivateEmployee(int id)
        {
            var employee = await _dbcontext.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (employee == null) return false;
            employee.IsActive = true;
            _dbcontext.SaveChanges();
            return true;
        }
        public async Task<bool> ReactivateDepartment(int id)
        {
            var department = await _dbcontext.Department.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (department == null) return false;
            department.IsActive = true;
            _dbcontext.SaveChanges();
            return true;
        }
        public async Task<int?> GetCountRoleWise(int role)
        {
            int? count = 0;
            if (role == 0)
            {
                count = await _dbcontext.Employees
                              .Where(e => e.Role == EmployeeRole.Employee && e.IsActive)
                              .CountAsync();
            }
            else if (role == 1)
            {
                count = await _dbcontext.Employees
                             .Where(e => e.Role == EmployeeRole.Admin && e.IsActive)
                             .CountAsync();
            }
            else if (role == 2)
            {
                count = await _dbcontext.Employees
                             .Where(e => e.Role == EmployeeRole.SuperAdmin && e.IsActive)
                             .CountAsync();
            }
            else
            {
                return null;
            }
            return count;
        }


    }
}
