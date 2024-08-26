using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using ManagementAPIDepartment;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class PaginatedServices : IPaginatedService
    {
        private readonly dbContext _dbContext;
        public PaginatedServices(dbContext db)
        {
            _dbContext = db;
        }


        public IQueryable<GetEmployeeDto>? ApplyFilering(IQueryable<GetEmployeeDto>? employee, string? filterOn,
            string? filterQuery)
        {
            if (string.IsNullOrEmpty(filterOn) == false &&
                    string.IsNullOrEmpty(filterQuery) == false)
            {
                // filer according to name or subpart of name
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    employee = employee.Where(e => e.Name.Contains(filterQuery));
                }

                // filter according to department or subpart of department name
                else if (filterOn.Equals("DepartmentName", StringComparison.OrdinalIgnoreCase))
                {
                    employee = employee.Where(e => e.DepartmentName.Contains(filterQuery));
                }

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

/*                        if (matchingRole == EmployeeRole.Employee)
                        {*/
                           employee = employee.Where(e => e.Role == matchingRole);
                       /* }
                        else if(matchingRole == EmployeeRole.) {
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
                if (SortBy.Equals("DepartmentId", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.DepartmentId) :
                          employee.OrderByDescending(e => e.DepartmentId);
                }
                if (SortBy.Equals("AdminId", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.AdminId) :
                          employee.OrderByDescending(e => e.AdminId);
                }
                /* if (SortBy.Equals("IsActive", StringComparison.OrdinalIgnoreCase))
                 {
                     employee = IsAscending ? employee.OrderBy(e => e.IsActive) :
                           employee.OrderByDescending(e => e.IsActive);
                 }*/
            }
            return employee;
        }

        public IQueryable<DepartmentDtos?> ApplyFileringDepartment(IQueryable<DepartmentDtos?> department, string? filterOn, string? filterQuery)
        {
            if (string.IsNullOrEmpty(filterOn) == false &&
                    string.IsNullOrEmpty(filterQuery) == false)
            {
                // filer according to name or subpart of name
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    department = department.Where(e => e.Name.Contains(filterQuery));
                }


            }
            return department;
        }
        public IQueryable<DepartmentDtos?> ApplySortingOnDepartment(IQueryable<DepartmentDtos?> department,
            string? SortBy, bool IsAscending)
        {
            if (string.IsNullOrEmpty(SortBy) == false)
            {
                if( SortBy.Equals( "Name" , StringComparison.OrdinalIgnoreCase))
                {
                    department = IsAscending ? department.OrderBy(e => e.Name) :
                        department.OrderByDescending(e => e.Name);
                }
                if (SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    department = IsAscending ? department.OrderBy(e => e.CreatedOn) :
                        department.OrderByDescending(e => e.CreatedOn);
                }
                /*if (SortBy.Equals("UpdatedOn", StringComparison.OrdinalIgnoreCase))

                {
                    department = IsAscending ? department.OrderBy(e => e.UpdatedOn) :
                        department.OrderByDescending(e => e.UpdatedOn);
                }*/
            }
            return department;
        }


        public async Task<List<GetEmployeeDto>> GetAllEmployee(PaginatedGetDto dto)
        {
            string? filterOn = "";
            if (dto.filterOn == null || dto.filterOn == string.Empty)
            {
                filterOn = "Name";
            }
            else filterOn = dto.filterOn;
            string? filterQuery = dto.filterQuery;
            string? SortBy = dto.SortBy;
            bool IsAscending = dto.IsAscending;
            int pageNumber = dto.pageNumber == 0 ? 1 : dto.pageNumber;
            int pageSize = dto.pageSize == 0 ? 10 : dto.pageSize;

            try
            {
                var employee = _dbContext.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Admin).
                    Where(e => e.IsActive == true)
                    .Select(e => new GetEmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Salary = e.Salary,
                        Role = e.Role,
                        DepartmentId = e.DepartmentId,
                        AdminId = e.AdminId,
                        DepartmentName = e.Department != null ? e.Department.Name : null,
                        AdminName = e.Admin != null ? e.Admin.Name : null,
                        CreatedOn = e.CreatedOn,
                        CreatedBy = e.Creator.Name
                       
                      


                    }).AsQueryable();

                //
                // Filtering
                employee = ApplyFilering(employee, filterOn, filterQuery);
                //

                //Sorting 
                employee = ApplySorting(employee, SortBy, IsAscending);
                //

                //Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                //

                return await employee.Skip(skipResults).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        
        public async Task<List<DepartmentDtos>?> GetAllDepartment(PaginatedGetDto dto)
        {
            string? filterOn = "";
            if (dto.filterOn == "" || dto.filterOn == null || dto.filterOn == string.Empty )
            {
                filterOn = "Name";
            }
            else filterOn = dto.filterOn;
            string? filterQuery = dto.filterQuery;
            string? SortBy = dto.SortBy;
            bool IsAscending = dto.IsAscending;
            int pageNumber = dto.pageNumber == 0 ? 1 : dto.pageNumber;
            int pageSize = dto.pageSize == 0 ? 10 : dto.pageSize;
            //
            var department = _dbContext.Department.Where(d => d.IsActive == true)
                 .Select(d => new DepartmentDtos
                 {
                     Name = d.Name,
                     Id = d.Id,
                    
                     CreatedOn = d.CreatedOn,
                     UpdatedOn = d.UpdatedOn
                 }).AsQueryable();

            // filtering
            department = ApplyFileringDepartment(department, filterOn, filterQuery);
            // Apply Sorting 
            department = ApplySortingOnDepartment( department , SortBy , IsAscending);
            // Applying Pagination
            var skipResult = ( pageNumber - 1) * pageSize;
            return await department.Skip(skipResult).Take(pageSize).ToListAsync();
        }
        public async Task<List<GetEmployeeDto>?> GetAllManagers(PaginatedGetDto dto)
        {
            string? filterOn = dto.filterOn;
            string? filterQuery = dto.filterQuery;
            string? SortBy = dto.SortBy;
            bool IsAscending = dto.IsAscending;
            int pageNumber = dto.pageNumber == 0 ? 1 : dto.pageNumber;
            int pageSize = dto.pageSize == 0 ? 10 : dto.pageSize;

            
            var managers =  _dbContext.Employees
                .Where(e => e.IsActive && _dbContext.Employees
                .Any(emp => emp.AdminId == e.Id))
                .Select(e => new GetEmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DepartmentId = e.DepartmentId,
                    AdminId = e.AdminId,
                    AdminName = e.Admin.Name,
                    DepartmentName = e.Department.Name,
                    Role = e.Role,
                  /*  CreatedBy = e.CreatedBy,*/
                    CreatedOn = e.CreatedOn,
                  
                }).Distinct().AsQueryable();
            //filtering  
            managers = ApplyFilering( managers, filterOn, filterQuery);

            //Sorting
            managers = ApplySorting(managers, SortBy, IsAscending);
            //Pagination
            var skipResult = (pageNumber - 1) * pageSize;
            return await managers.Skip(skipResult).Take(pageSize).ToListAsync();
        }
    }
}
