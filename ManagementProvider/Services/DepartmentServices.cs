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
namespace ManagementAPI.Provider.Services
{
    public class DepartmentServices : IDepartmentServices
    {
        private readonly dbContext _dbContext;
        public DepartmentServices(dbContext db)
        {
            _dbContext = db;
        }
        public async Task<List<DepartmentDtos>?> GetDepartment()
        {
            try
            {
                var department = await _dbContext.Department
                    .Where(e => e.IsActive == true)
                    .Select(e => new DepartmentDtos
                    { 
                    Name = e.Name,
                    Id = e.Id,
                    CreatedBy = e.CreatedBy,
                    CreatedOn = e.CreatedOn,
                    UpdatedBy = e.UpdatedBy,
                    UpdatedOn = e.UpdatedOn,
                    }).ToListAsync();
                return department;
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
                var department = await _dbContext.Department.Where(e => e.Name == addDepartmentDtos.Name.ToUpper()).FirstOrDefaultAsync();

                if (department != null && department.IsActive == true) { return -1; }
                if (department != null && department.IsActive == false)
                {
                    department.IsActive = true;
                    department.UpdatedOn = DateTime.Now;
                    department.UpdatedBy = createdBy;
                    await _dbContext.SaveChangesAsync();
                    return department.Id;
                }
                var Department = new Department { Name = addDepartmentDtos.Name.ToUpper() };
                Department.CreatedBy = createdBy;
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
        public async Task<List<EmployeeDto>> GetEmployeeDetails( int id)
        {
            try
            {
                var department = await _dbContext.Department.FirstOrDefaultAsync(e => e.Id == id);
                if (department == null || !department.IsActive) return null;
                var employee = await _dbContext.Employees.Include(e => e.Admin).Include(e => e.Department).
                    Where(e => e.IsActive == true && e.DepartmentId == id).Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Salary = e.Salary,
                        Role = e.Role,
                        AdminId = e.AdminId,
                        AdminName = e.Admin != null ? e.Admin.Name : null,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department != null ? e.Department.Name : null,
                        CreatedOn = e.CreatedOn,
                        CreatedBy = e.CreatedBy,
                        UpdatedOn = e.UpdatedOn,
                        UpdatedBy = e.UpdatedBy,
                    }).ToListAsync();
                if (employee == null) return null;
                return employee;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message); 
            }
        }
    }
}
