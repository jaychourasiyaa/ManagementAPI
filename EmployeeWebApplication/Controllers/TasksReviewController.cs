using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Mvc;
namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksReviewController : ControllerBase
    {
        private readonly dbContext _dbContext;
        private readonly ITaskReviewServices taskReviewServices;
        public TasksReviewController(dbContext db, ITaskReviewServices taskReviewServices)
        {
            _dbContext = db;
            this.taskReviewServices = taskReviewServices;
        }
        [HttpGet]
        public ActionResult<GetTaskReviewDto> Get()
        {
            var taskReview = taskReviewServices.GetTaskReview();
            return Ok(taskReview);
        }
        [HttpPost]
        public ActionResult<int> Post(AddTaskReviewDto taskReviewDtos)
        {
            int tasksReviewId = taskReviewServices.AddTaskReview(taskReviewDtos);
            return Ok(tasksReviewId);
        }
       
        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(int id)
        {

            return Ok(taskReviewServices.DeleteTaskReview(id));
        }
    }
}

