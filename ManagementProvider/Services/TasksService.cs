using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using TasksAPI;
using ManagementAPI.Contract.Dtos;
using Microsoft.EntityFrameworkCore;
using ManagementAPIEmployee;
using ManagementAPI.Provider.Migrations;
using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ManagementAPI.Contract.Enums;
using ManagementAPI.Contract.Responses;
using Employee_Role;
using Microsoft.Identity.Client;
using Taask_status;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace ManagementAPI.Provider.Services
{
    public class TasksService : ITasksServices
    {
        private readonly dbContext _dbContext;
         
        public TasksService( dbContext db)
        {
            _dbContext = db;
            
        }
        /* public async Task<int> AssignSelfTask(AddTasksDtos dto, int assignedBy)
         {
             var tasks = new Tasks
             {
                 Name = dto.Name,
                 AssignedById = assignedBy,
                 AssignedToId = assignedBy,
                 Description = dto.Description,
                 CreatedBy = assignedBy,
                 Status = dto.Status
             };
             await _dbContext.Taasks.AddAsync(tasks);
             await _dbContext.SaveChangesAsync();    
             return tasks.Id;
         }*/
        public IQueryable<GetTaskDto>? ApplyFilering(IQueryable<GetTaskDto>? tasks, string filterOn,
             int assignedTo)
        {

            if (string.IsNullOrEmpty(filterOn) == false)
            {
             
                if (filterOn.Equals("AssignedToId", StringComparison.OrdinalIgnoreCase))
                {
                    tasks = tasks.Where(t => t.AssignedById == assignedTo ||t.AssignedToId == assignedTo);
                }

            }
            return tasks;
        }
        public async Task<bool> CheckManagerOfEmployee(int managerId, int employeeId)
        {
            var manager = await _dbContext.Employees.Where(e => e.Id == employeeId
                                                     && e.AdminId == managerId).FirstOrDefaultAsync();
            if (manager == null) return false;
            return true;
        }
        public async Task<List<GetTaskDto>> GetAllTasks(EmployeeRole Role, int assignedTo)
        {
            var tasks =  _dbContext.Taasks.Include(e => e.AssignedBy).Include(e => e.AssignedTo)
             .Select(e => new GetTaskDto
             {
                Id = e.Id,
                Name = e.Name,
                Assigned_From = e.AssignedBy.Name, 
                Assigned_To = e.AssignedTo.Name,
                AssignedById = e.AssignedById,
                AssignedToId = e.AssignedToId,
                CreatedOn = e.CreatedOn,
                CreatedBy = e.CreatedBy,
                UpdatedBy = e.UpdatedBy,
                UpdatedOn = e.UpdatedOn,
                Description = e.Description,
                Status = e.Status,

            }).AsQueryable();
            if( Role == EmployeeRole.SuperAdmin)
            { 
                return await tasks.ToListAsync(); 
            }
           tasks = ApplyFilering( tasks, "AssignedToId", assignedTo  );
            return await tasks.ToListAsync();
        }
        public async Task <int> AddTasks(AddTasksDtos dtos, int assignedBy)
        {
            var manager = await _dbContext.Employees.Where(e => e.Id == assignedBy).FirstOrDefaultAsync();
            if (manager == null || !manager.IsActive) 
            { 
                return -1;
            }
            var employee = await _dbContext.Employees.Where( e=> e.Id == dtos.AssignedToId).FirstOrDefaultAsync();
            if (employee == null || !employee.IsActive)
            { 
                return -2; 
            }

            if (manager.Role != EmployeeRole.SuperAdmin && dtos.AssignedToId != assignedBy)
             {
                bool check = await CheckManagerOfEmployee(manager.Id, employee.Id);
                if( !check )
                {
                    return -3;
                }
               
             }
            
           
          
            var tasks = new Tasks
            {
                Name = dtos.Name,
                AssignedById = assignedBy,
                AssignedToId = dtos.AssignedToId,
                Description = dtos.Description,
                Status= dtos.Status,
                CreatedBy = assignedBy
            };
            await _dbContext.Taasks.AddAsync(tasks);
            await _dbContext.SaveChangesAsync();
            return tasks.Id;
        }
        public async Task<int> UpdateTasks(int status, int id ,int AccessingId)
        {
            var tasks = await _dbContext.Taasks.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (tasks == null || !tasks.IsActive) return -1;
            if (tasks.AssignedById != AccessingId && tasks.AssignedToId != AccessingId) return -2;
         
            if(status<0 ||status >3 ) { return -3; }
            if (status == 0)
            {
                tasks.Status = TasksStatus.Pending;
            }

            else  if (status == 1)
                {
                    tasks.Status = TasksStatus.Running;
                }
            else   if (status == 2)
                {
                    tasks.Status = TasksStatus.Pending;
                }
             else  
                {
                    tasks.Status = TasksStatus.Complete;
                }
               await _dbContext.SaveChangesAsync();
            return 1 ;
        }
        public async Task<bool> DeleteTasks(int id , int AccessingId)
        {
            var tasks = await _dbContext.Taasks.FirstOrDefaultAsync( e=> e.Id == id);
            if( tasks.AssignedById != AccessingId && tasks.AssignedToId != AccessingId) return false;   
            if (tasks == null || tasks.IsActive) return false;
            tasks.IsActive = false;
            _dbContext.SaveChanges();
            return true;

        }
       public async Task<List<TaskDtos>?> GetTaskById(int assigneToId )
        {
            var tasksList = await _dbContext.Taasks
                .Where( t=> t.AssignedToId == assigneToId || t.AssignedById == assigneToId)
                .Select( e=> new TaskDtos
                {
                    Id = e.Id,
                    Name= e.Name,
                    Description=e.Description,
                    AssignedById = e.AssignedById,
                    AssignedToId = e.AssignedToId,
                    Assigned_From = e.AssignedBy.Name,
                    Assigned_To = e.AssignedTo.Name,
                    CreatedOn =e.CreatedOn,
                    Status = e.Status,
                    
                 }).ToListAsync();
            if (tasksList == null) return null;
            return tasksList;
        }

        
    }
}
