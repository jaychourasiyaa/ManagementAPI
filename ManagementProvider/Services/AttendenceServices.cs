/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Models;
using ManagementAPI.Provider.Database;
using ManagementAPI.Provider.Services;
using ManagementAPIEmployee;
using Microsoft.EntityFrameworkCore;
using ManagementAPI.Contract.Enums;
using Microsoft.Identity.Client;

public class AttendenceServices : IAttendenceServices
{
    private readonly dbContext _dbContext;

    public AttendenceServices(dbContext db)
    {
        _dbContext = db;
    }
    public async Task <int> AttendenceAdd(AddAttendenceDtos adtos)
    {
        try
        {
            // checking if employee exists or not
            var employee = await _dbContext.Employees
                .Where(e => e.Id == adtos.EmployeeId && e.IsActive == true)
                .FirstOrDefaultAsync();

            // if employee does not exist their attendence cannot be added
            if (employee == null) return -1;

            // making instance of attendence 
            var attendence = new Attendence
            {
                EmployeeId = employee.Id,
                Status = adtos.Status,
            };
            // adding and save chages returning attendence id
            await _dbContext.Attendences.AddAsync(attendence);
            await _dbContext.SaveChangesAsync();
            return attendence.Id;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<List<GetAttendenceDto>> getAttendenceList(int id)
    {
        try
        {
            // checking if employee exists or not
            var employee = await _dbContext.Employees
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            // if employee does not exist returning null

            if (employee == null) return null;

            //fetching  list of attendence from database 
            var attendenceList = await _dbContext.Attendences
                .Where(e => e.EmployeeId == id)
                .Select(e => new GetAttendenceDto
                {
                    date = e.Date,
                    status = e.Status
                }
                ).ToListAsync();
            return attendenceList;
        }
        catch (Exception ex)
        {
            throw new Exception ( ex.Message);
        }
    }
    

}
*/