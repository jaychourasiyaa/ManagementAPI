using ManagementAPI.Contract.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksReviewAPI;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ITaskReviewServices
    {
        public int AddTaskReview(AddTaskReviewDto tasksReviewDtos);
        public List<GetTaskReviewDto> GetTaskReview();
      /*  public TasksReview UpdateTaskReview(AddTaskReviewDto tasksReviewDtos);*/
        public bool DeleteTaskReview(int id);


    }
}
