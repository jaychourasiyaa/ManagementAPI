using ManagementAPI.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Provider.Database;
using TasksReviewAPI;
using ManagementAPI.Contract.Dtos;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ManagementAPIEmployee;
using Employee_Role;
using Microsoft.Identity.Client;
using Taask_status;
namespace ManagementAPI.Provider.Services
{
    public class TaskReviewServices : ITaskReviewServices
    {
        private readonly dbContext _dbContext;

        public TaskReviewServices(dbContext db) { _dbContext = db; }
        public async Task<List<GetTaskReviewDto>?> GetTaskReview(EmployeeRole Role, int AccessingId)
        {

            try
            {
                // getting list of reviews for a task 
                var tasksreview =   _dbContext.TasksReviews.Include( e=> e.Reviewer).Include(e => e.Tasks)
                                    .Select(e => new GetTaskReviewDto
                                    {
                                        Id = e.Id,
                                        TaskId = e.Tasks.Id,
                                        ReviewById = e.Reviewer.Id,
                                        ReviewerName = e.Reviewer.Name,
                                        Comments = e.Comments,
                                        AssignedToId = e.Tasks.AssignedToId,
                                        AssignedById = e.Tasks.AssignedById,    
                                        
                                       
                                    }).AsQueryable();

                // if employee role is  not superadmin filtering tasks
                if (Role != EmployeeRole.SuperAdmin)
                {
                    tasksreview = tasksreview.Where(t => t.AssignedToId == AccessingId || t.AssignedToId == AccessingId);
                }
                return await tasksreview.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int?> AddTaskReview(AddTaskReviewDto tasksReviewDtos, EmployeeRole Role, int accessingId)
        {
            try
            {
                // checking tasks to be reviewed exists or not
                var tasks = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasksReviewDtos.TasksId);

                if (tasks == null || tasks.IsActive == false) return -1;

                // only legit person can reveiw the task ,whether a superadmin or the task assigner or task assigni
                if (Role != EmployeeRole.SuperAdmin)
                {
                    if (tasks.AssignedToId != accessingId && tasks.AssignedById != accessingId)
                    {
                        return -2;
                    }
                }
                var tasksReview = new TasksReview
                {
                    TasksId = tasksReviewDtos.TasksId,
                    ReviewBy = accessingId,
                    Comments = tasksReviewDtos.Comments,
                    CreatedBy = accessingId,
                    
                };
                await _dbContext.AddAsync(tasksReview);
                await _dbContext.SaveChangesAsync();
                return tasksReview.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<bool> DeleteTaskReview(int aceessingId , int reviewId)
        {
            try
            {
                var typeess = TasksStatus.Completed.ToString();
                Console.WriteLine(typeess);
                var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == aceessingId);
                if (employee == null || employee.IsActive == false)
                {
                    return false;
                }
                var taskReview = _dbContext.TasksReviews.Include(t => t.Tasks).FirstOrDefault(t => t.Id == reviewId);
                if (taskReview == null) return false;
                if (employee.Role != EmployeeRole.SuperAdmin)
                {
                    if (taskReview.Tasks.AssignedById != aceessingId && taskReview.Tasks.AssignedToId != aceessingId)
                    {
                        return false;
                    }
                }
                 _dbContext.TasksReviews.Remove(taskReview);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
