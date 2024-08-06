using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Models;
using Microsoft.EntityFrameworkCore;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
namespace ManagementAPI.Provider.Services
{
    public class SalaryServies : ISalaryServices
    {
        private readonly dbContext _dbContext;
        public SalaryServies( dbContext db)
        {
            _dbContext = db;
        }
        public async Task<bool> CheckEmployee( int  employeeId )
        {
            try
            {
                var employee = await _dbContext.Employees.Where(e => e.Id == employeeId).FirstOrDefaultAsync();
                if (employee == null || !employee.IsActive)
                {
                    return false;
                }
                return true;
            }
            catch ( Exception ex )
            {
                throw ( new Exception( ex.Message ) );  
            }
        }
        public async Task<List<DateTime>> getById(int id)
        {
            try
            {
                bool check = await CheckEmployee(id);
                if (!check)
                {
                    return null;
                }
                var salaries = await _dbContext.Salaries.Where(s => s.EmployeeId == id)
                    .Select(s => s.date).ToListAsync();
                return salaries;
            }
            catch ( Exception ex)
            {
                throw ( new Exception( ex.Message ) );
            }
        }
        public async Task<int> TakeAdvance( SalaryDtos dtos)
        {

            try
            {
                bool check = await CheckEmployee(dtos.EmployeeId);
                if (!check)
                {
                    return -1;
                }
                var alreadyTaken = await _dbContext.Salaries.Where(
                    e => e.EmployeeId == dtos.EmployeeId
                    && e.month == DateTime.Now.Month
                    && e.year == DateTime.Now.Year).FirstOrDefaultAsync();

                if (alreadyTaken != null) return -2;

                var salary = new Salary { EmployeeId = dtos.EmployeeId };
                _dbContext.Salaries.Add(salary);
                _dbContext.SaveChanges();
                return salary.Id;
            }
            catch ( Exception ex) 
            { 
                throw ( new Exception(ex.Message ) );
            }
        }
    }
}
