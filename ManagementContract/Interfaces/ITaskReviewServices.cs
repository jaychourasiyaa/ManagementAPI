using Employee_Role;
using ManagementAPI.Contract.Dtos.TaskReviewDtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksReviewAPI;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ITaskReviewServices
    {
        public Task<List<GetTaskReviewDto>?> GetTaskReview(int id);
        public Task<int?> AddTaskReview(AddTaskReviewDto tasksReviewDtos );
        public Task<bool> UpdateTaskReview(int TaskId, int CommentId, string Comment);
        public Task<bool> DeleteTaskReview(int id);
       

    }
}
