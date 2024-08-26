using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPIDepartment;
using ManagementAPI.Contract.Dtos;
using ManagementAPIDepartment;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.ComponentModel;
using Taask_status;
namespace ManagementAPI.Provider.Services
{
    public class DepartmentServices : IDepartmentServices
    {
        private readonly dbContext _dbContext;
        public DepartmentServices(dbContext db)
        {
            _dbContext = db;
        }
        public IQueryable<GetDepartmentDtos?> ApplyFileringDepartment(IQueryable<GetDepartmentDtos?> department, string? filterOn, string? filterQuery ,DateTime ? startDate, DateTime? endDate)
        {
            if( startDate != null && endDate != null )
            {
                department = department.Where( d => d.CreatedOn >= startDate && d.CreatedOn <= endDate );
            }
            if (string.IsNullOrEmpty(filterQuery) == false)
            {

                department = department.Where(e => e.Name.Contains(filterQuery));
                // filer according to name or subpart of name
                /*if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    department = department.Where(e => e.Name.Contains(filterQuery));
                }*/
                /* // filer according to name or subpart of name
                 if (filterOn.Equals("CreatedBy_Name", StringComparison.OrdinalIgnoreCase))
                 {
                     department = department.Where(e => e.CreatedBy_Name.Contains(filterQuery));
                 }*/
            }
            return department;
        }
        public IQueryable<GetDepartmentDtos?> ApplySortingOnDepartment(IQueryable<GetDepartmentDtos?> department,
            string? SortBy, bool IsAscending)
        {
            if (string.IsNullOrEmpty(SortBy) == false)
            {
                if (SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    department = IsAscending ? department.OrderBy(e => e.Name) :
                        department.OrderByDescending(e => e.Name);
                }
                if (SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    department = IsAscending ? department.OrderBy(e => e.CreatedOn) :
                        department.OrderByDescending(e => e.CreatedOn);
                }
                if (SortBy.Equals("CreatedBy_Name", StringComparison.OrdinalIgnoreCase))

                {
                    department = IsAscending ? department.OrderBy(e => e.CreatedBy_Name) :
                        department.OrderByDescending(e => e.CreatedBy_Name);
                }
            }
            return department;
        }
        public async Task<List<GetDepartmentDtos>?> GetDepartment(PaginatedGetDto dto)
        {
            try
            {
                var typeess = TasksStatus.Completed.ToString();
                Console.WriteLine(typeess);
                string? filterOn = "Name";
                string? filterQuery = dto.filterQuery;
                string? SortBy = dto.SortBy;
                bool IsAscending = dto.IsAscending;
                int pageNumber = dto.pageNumber <=0 ? 1 : dto.pageNumber;
                int pageSize = dto.pageSize <= 0 ? 10 : dto.pageSize;
                DateTime? startDate = dto.startDate;
                DateTime? endDate = dto.endDate;
                // getting all department in a list

                var department = _dbContext.Department
                    .Include(e => e.Employee)
                    .Where(e => e.IsActive == true)
                    .Select(e => new GetDepartmentDtos
                    {
                        Id  = e.Id,
                        Name = e.Name,
                        CreatedOn = e.CreatedOn,
                        CreatedBy_Name = e.Employee.Name
                        
                    }).AsQueryable();
                if( dto.filterOn == "" && dto.filterQuery == "" && 
                    dto.SortBy == "" &&  dto.IsAscending == true &&
                    dto.pageNumber == -1 && dto.pageSize == -1 )
                {
                    return await department.ToListAsync();
                }
                
                department = ApplyFileringDepartment(department, filterOn, filterQuery ,startDate , endDate);
                // Apply Sorting 
                department = ApplySortingOnDepartment(department, SortBy, IsAscending);
                // Applying Pagination
                var skipResult = (pageNumber - 1) * pageSize;
                return await department.Skip(skipResult).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception ( ex.Message);
            }
        }
        public async Task<int> AddDepartment(AddDepartmentDtos addDepartmentDtos , int createdBy)
        {
            try
            {
                //checking if department exists or not
                var department = await _dbContext.Department.Where(e => e.Name == addDepartmentDtos.Name.ToUpper()).FirstOrDefaultAsync();


                // if department already exists 
                if (department != null && department.IsActive == true) { return -1; }
                 
                // if department is soft deleted  updating isActive status to true
                if (department != null && department.IsActive == false)
                {
                    department.IsActive = true;
                    department.UpdatedOn = DateTime.Now;
                    department.UpdatedBy = createdBy;
                    await _dbContext.SaveChangesAsync();
                    return department.Id;
                }

                // adding fresh new department
                var Department = new Department 
                { Name = addDepartmentDtos.Name.ToUpper() ,
                 CreatedBy = createdBy
                };
               
                await _dbContext.Department.AddAsync(Department);
                await _dbContext.SaveChangesAsync();
                return Department.Id;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            /*public async Task<bool> UpdateDepartment( int id , bool isactive)
            {
                var department = await _dbContext.Department.FindAsync(id);
                if( department == null ) return false;
                department.IsActive = isactive;
                _dbContext.SaveChanges();
                return true;
            }*/
        }
        public async Task<bool> DeleteDepartment( int id , int deletedBy)
        {
            try
            {
                // finding department and if exists soft deleting and updating , updatedOn and updatedBy
                var department = await _dbContext.Department.FindAsync(id);
                if (department == null || !department.IsActive) return false;
                department.IsActive = false;
                department.UpdatedOn = DateTime.Now;
                department.UpdatedBy = deletedBy;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // get employee deparment wise
        public async Task<List<IdandNameDto>?> GetEmployeeDetails( int id)
        {
            // funtion returns the employee department wise
            try
            {
                // checking if department exists or not
                var department = await _dbContext.Department.FirstOrDefaultAsync(e => e.Id == id);

                // if not exists return null
                if (department == null || !department.IsActive) return null;

                //fetching all employee for that department
                var employee = await _dbContext.Employees.
                    Where(e => e.IsActive == true && e.DepartmentId == id).Select(e => new IdandNameDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                    }).ToListAsync();

                // if no employee exists returning null
                if (employee.Count ==0) return null;
                return employee;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message); 
            }
        }
        public async Task<int?> UpdateDepartment( IdandNameDto dto ,int updatedBy)
        {
            var department = await _dbContext.Department
                .Where( d=> d.Id == dto.Id )
                .FirstOrDefaultAsync();
            if( department == null)
            {
                return -1;
            }
            var alreadyExistsDept = await _dbContext.Department.Where(e => e.Name == dto.Name.ToUpper()).FirstOrDefaultAsync();


            // if department already exists 
            if (alreadyExistsDept != null && alreadyExistsDept.IsActive == true && alreadyExistsDept.Id != dto.Id)
            {
                return -2; 
            }

        // updating department
            if (department != null)
            {
                department.Name = dto.Name.ToUpper();
                department.IsActive = true;
                department.UpdatedOn = DateTime.Now;
                department.UpdatedBy = updatedBy;
                await _dbContext.SaveChangesAsync();
                
            }
            return department.Id;

        }
        public async Task<List<IdandNameDto>?> GetDeletedDepartment()
        {
            var departments = await _dbContext.Department
                .Where(d => d.IsActive == false)
                .Select(d => new IdandNameDto
                {
                    Id = d.Id,
                    Name = d.Name,
                }).ToListAsync();
            if (departments == null)
                return null;
            return departments;
        }
        public async Task<bool> ReactivateDepartment(int id)
        {
            var department = await _dbContext.Department.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (department == null) return false;
            department.IsActive = true;
            _dbContext.SaveChanges();
            return true;
        }
        public async Task<IdandNameDto> GetDepartmentById(int id)
        {
            var department = await _dbContext.Department
                .Where(d => d.Id == id && d.IsActive)
                .Select( d => new IdandNameDto {  Id = id , Name = d.Name}).FirstOrDefaultAsync();
            if (department == null) return null;
            return department;
        }

    }
}
