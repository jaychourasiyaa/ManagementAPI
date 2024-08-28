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
using ManagementAPI.Contract.Models;
namespace ManagementAPI.Provider.Services
{
    public class TaskReviewServices : ITaskReviewServices 
    {
        private readonly dbContext _dbContext;
        private readonly ITasksServices tasksServices;
        public TaskReviewServices(dbContext db, ITasksServices ts)
        {
            _dbContext = db;
            tasksServices = ts;
        }
        public async Task<List<GetTaskReviewDto>?> GetTaskReview(EmployeeRole Role, int AccessingId)
        {

            try
            {
                // getting list of reviews for a task 
                var tasksreview = _dbContext.TasksReviews.Include(e => e.Reviewer).Include(e => e.Tasks)
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

        public async Task<int?> AddTaskReview(AddTaskReviewDto tasksReviewDtos, EmployeeRole Role, int accessingId )
        {
            try
            {
                var accessor =await _dbContext.Employees.Where(e=> e.Id ==  accessingId && e.IsActive).FirstOrDefaultAsync();
                if (accessor == null)
                {
                    return -1;
                }
                // checking tasks to be reviewed exists or not
                var tasks = await _dbContext.Taasks.FirstOrDefaultAsync(t => t.Id == tasksReviewDtos.TasksId);

                if (tasks == null || tasks.IsActive == false)
                { 
                    return -2;
                }
                var project = await _dbContext.Projects.Where(p => p.Id == tasks.ProjectId)
                    .FirstOrDefaultAsync();
                if( project == null)
                {
                    return -3;
                }
                // only legit person can reveiw the task ,whether a superadmin or the task assigner or task assigni or the manager or assigni or team member of same project
                int checkAccess = await tasksServices.CheckTaskAccess(Role,tasks,accessingId,project);
                if( checkAccess < 0)
                {
                    return checkAccess;
                }
                /*if (Role != EmployeeRole.SuperAdmin)
                {
                    if (tasks.AssignedToId != accessingId && tasks.AssignedById != accessingId)
                    {
                        int assingedToId = Convert.ToInt32(tasks.AssignedToId);
                        bool checkManager = await tasksServices.CheckManagerOfEmployee(assingedToId, accessingId);
                        bool checkTeamMember = await tasksServices.CheckTeamMemberOfProject(accessingId, assingedToId, project.Id);
                    }
                }*/
                var tasksReview = new TasksReview
                {
                    TasksId = tasksReviewDtos.TasksId,
                    ReviewBy = accessingId,
                    Comments = tasksReviewDtos.Comments,
                    CreatedBy = accessingId,

                };
                var log = new Log
                {
                    TaskId = tasks.Id,
                    Message = $"{accessor.Name} Added Comments in task Id: {tasks.Id} Name : {tasks.Name}"
                };
                await _dbContext.Logs.AddAsync(log);
                await _dbContext.AddAsync(tasksReview);
                await _dbContext.SaveChangesAsync();
                return tasksReview.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<bool> DeleteTaskReview(int aceessingId, int reviewId)
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
                    if (taskReview.ReviewBy!= aceessingId)
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
        public async Task<bool> UpdateTaskReview(int accessingId, int TaskId, int CommentId, string comments, string Role)
        {
            try
            {
                var accessor = await _dbContext.Employees.FirstOrDefaultAsync( e=> e.Id == accessingId || e.IsActive);
                if( accessor == null ) 
                { 
                    return false;
                }
                var taskReview = await _dbContext.TasksReviews.Where(t => t.Id == CommentId && t.TasksId == TaskId)
                    .FirstOrDefaultAsync();

                if (taskReview == null)
                {
                    return false;
                }
                if ( Role != "SuperAdmin")
                {
                    if (accessingId != taskReview.ReviewBy)
                    {
                        return false;
                    }
                }
                string previousComment = taskReview.Comments;
                taskReview.Comments = comments;
                if( previousComment != taskReview.Comments )
                {
                    var log = new Log
                    {
                        TaskId = TaskId,
                        Message = $"{accessor.Name} Updated Comment from {previousComment} to {taskReview.Comments} "

                    };
                    await _dbContext.Logs.AddAsync(log);
                }
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
