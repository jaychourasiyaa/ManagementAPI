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
namespace ManagementAPI.Provider.Services
{
    public class TaskReviewServices : ITaskReviewServices
    {
        private readonly dbContext _dbContext;
       
        public TaskReviewServices(dbContext db) { _dbContext = db; }
        public List<GetTaskReviewDto> GetTaskReview( )
        {


             /*public int Id { get; set; }
        public string Name { get; set; }
        public string Assigned_From { get; set; }
        public string Assigned_To { get; set; }
        public int AssignedById { get; set; }
        public int AssignedToId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public int ReviewById { get; set; }
        public string ReviewName
        {
            get; set;
        }*/
            var tasksreview = _dbContext.TasksReviews.Include(e => e.Tasks).Include(e => e.Reviewer)
                                .Select(e => new GetTaskReviewDto
                                {
                                    Id = e.Id,
                                    TaskId = e.Tasks.Id,
                                    ReviewerName = e.Reviewer.Name,
                                    Name = e.Tasks.Name,
                                    Assigned_From = e.Tasks.AssignedBy.Name,
                                    Assigned_To = e.Tasks.AssignedTo.Name,
                                    AssignedById = e.Tasks.AssignedById,
                                    AssignedToId = e.Tasks.AssignedToId,
                                    Description = e.Tasks.Description,
                                    CreatedOn = e.CreatedOn,
                                    Comments = e.Comments,
                                    Status = e.Tasks.Status,
                                }).ToList();
            return tasksreview;
        }
       
        public int AddTaskReview(AddTaskReviewDto tasksReviewDtos  )
        {
            var tasksReview = new TasksReview
            {
                TasksId = tasksReviewDtos.TasksId,
                ReviewBy = tasksReviewDtos.ReviewById,
                Comments = tasksReviewDtos.Comments,
            };
            _dbContext.Add(tasksReview);
            _dbContext.SaveChanges();
            return tasksReview.Id;
        }
        public TasksReview UpdateTaskReview(AddTaskReviewDto tasksReviewDtos,int id)
        {
            var tasksReview  = _dbContext.TasksReviews.Find(id);
            tasksReview.ReviewBy = tasksReviewDtos.ReviewById;    
            tasksReview.UpdatedOn = DateTime.Now;
            tasksReview.Comments = tasksReviewDtos.Comments;
            _dbContext.SaveChanges();
            return tasksReview;
        }
        public bool DeleteTaskReview( int id)
        {
            var tasksReviews = _dbContext.TasksReviews.Find( id);
            _dbContext.TasksReviews.Remove(tasksReviews);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
