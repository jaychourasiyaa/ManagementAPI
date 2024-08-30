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
        private readonly IJwtService JwtService;
        public TaskReviewServices(dbContext db, ITasksServices ts, IJwtService jwtservice)
        {
            _dbContext = db;
            tasksServices = ts;
            JwtService = jwtservice;
        }
        public async Task<List<GetTaskReviewDto>?> GetTaskReview(int id)
        {
            try
            {
                // getting list of reviews for a task 
                var tasksreview = _dbContext.TasksReviews
                    .Include(e => e.Reviewer)
                    .Include(e => e.Tasks)
                    .Where(e=> e.TasksId == id)
                    .Select(e => new GetTaskReviewDto
                    {
                        Id = e.Id,
                        ReviewerName = e.Reviewer.Name,
                        Comments = e.Comments,
                        dateTime = e.CreatedOn,
                    }).AsQueryable();

                // if employee role is  not superadmin filtering tasks
                /*if (Role != EmployeeRole.SuperAdmin)
                {
                    tasksreview = tasksreview.Where(t => t.AssignedToId == AccessingId || t.AssignedToId == AccessingId);
                }*/
                return await tasksreview.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int?> AddTaskReview(AddTaskReviewDto tasksReviewDtos)
        {
            try
            {
                // checking valid person is logged in
                var accessor = await _dbContext.Employees.Where(e => e.Id == JwtService.UserId && e.IsActive).FirstOrDefaultAsync();
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
                if (project == null)
                {
                    return -3;
                }

                // only legit person can reveiw the task
                // -- whether a superadmin
                // -- or the task assigner
                // -- or task assignee
                // -- or the manager of assignee
                // -- or team member of same project

                int checkAccess = await tasksServices.CheckTaskAccess(JwtService.UserRole, tasks, JwtService.UserId, project);
                // return positive value if accessible
                if (checkAccess < 0)
                {
                    return checkAccess;
                }

                // adding review in task
                var tasksReview = new TasksReview
                {
                    TasksId = tasksReviewDtos.TasksId,
                    ReviewBy = JwtService.UserId,
                    Comments = tasksReviewDtos.Comments,
                    CreatedBy = JwtService.UserId,

                };

                // add log for creating review in a task
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
        public async Task<bool> UpdateTaskReview(int TaskId, int CommentId, string comments)
        {
            try
            {
                // checking if legit person is logged in or not
                var accessor = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == JwtService.UserId || e.IsActive);
                if (accessor == null)
                {
                    return false;
                }

                // checking if task and taskreview id is valid or not
                var taskReview = await _dbContext.TasksReviews.Where(t => t.Id == CommentId && t.TasksId == TaskId)
                    .FirstOrDefaultAsync();
                if (taskReview == null)
                {
                    return false;
                }

                // checking if the task is accessible or not
                if (JwtService.UserRole != EmployeeRole.SuperAdmin)
                {
                    if (JwtService.UserId != taskReview.ReviewBy)
                    {
                        return false;
                    }
                }
                string previousComment = taskReview.Comments;
                taskReview.Comments = comments;

                // if done any changes in comment adding log for it
                if (previousComment != taskReview.Comments)
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
        public async Task<bool> DeleteTaskReview(int reviewId)
        {
            try
            {
                // checking if legit person is logged in
                var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == JwtService.UserId);
                if (employee == null || employee.IsActive == false)
                {
                    return false;
                }

                // task review to be deleted must be a valid review
                var taskReview = _dbContext.TasksReviews.Include(t => t.Tasks).FirstOrDefault(t => t.Id == reviewId);
                if (taskReview == null) return false;

                // checking task accessiblity
                if (employee.Role != EmployeeRole.SuperAdmin)
                {
                    if (taskReview.ReviewBy != JwtService.UserId)
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
