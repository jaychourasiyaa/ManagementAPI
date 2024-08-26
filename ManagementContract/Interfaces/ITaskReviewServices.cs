using Employee_Role;
using ManagementAPI.Contract.Dtos;
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
        public Task<int?> AddTaskReview(AddTaskReviewDto tasksReviewDtos, EmployeeRole Role, int accessingId);
        public Task<List<GetTaskReviewDto>?> GetTaskReview(EmployeeRole role, int accessingId);
      /*  public TasksReview UpdateTaskReview(AddTaskReviewDto tasksReviewDtos);*/
        public Task<bool> DeleteTaskReview(int accessingId ,int id);


    }
}
