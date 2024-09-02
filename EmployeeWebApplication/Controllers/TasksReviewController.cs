using Employee_Role;
using ManagementAPI.Contract.Dtos.TaskReviewDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Responses;
using ManagementAPI.Provider.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Security.Claims;
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRespones<List<GetTaskReviewDto>?>>> Get(int id)
        {
            var response = new ApiRespones<List<GetTaskReviewDto>?>();

            try
            {
                // calling service that fetches all reveiew of a task
                var taskReview = await taskReviewServices.GetTaskReview(id);
                response.Message = "TaskReview Fetcehd";
                response.Data = taskReview;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiRespones<int?>>> Add(AddTaskReviewDto taskReviewDtos)
        {
            var respones = new ApiRespones<int?>();
            try
            {
                var tasksReviewId = await taskReviewServices.AddTaskReview(taskReviewDtos);
                if (tasksReviewId == -1)
                {
                    respones.Message = "Task Id is invalid";
                    return NotFound(respones);
                }
                if (tasksReviewId == -2)
                {
                    respones.Message = "Task is unaccissble";
                    return BadRequest(respones);
                }
                respones.Message = "Comments Added";
                respones.Data = tasksReviewId;
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Success = false;
                respones.Message = ex.Message;
                return BadRequest(respones);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiRespones<bool>>> Update(UpdateTaskReview dto)
        {
            var response = new ApiRespones<bool>();
            try
            {
                var result = await taskReviewServices.UpdateTaskReview(dto.TaskId, dto.CommentId, dto.Comment);

                if (result == false)
                {
                    response.Message = "Task Review Id is invalid or inaccessible";
                    return NotFound(response);
                }
                response.Message = "Review Updated";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);

            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiRespones<bool>>> Delete(int id)
        {
            var respones = new ApiRespones<bool>();
            try
            {
                
                bool deleted = await taskReviewServices.DeleteTaskReview(id);
                if (!deleted)
                {
                    respones.Message = "Task Review Id is invalid or inaccessible";
                    respones.Data = false;
                    return NotFound(respones);
                }
                respones.Message = "TaskReview Deleted";
                return Ok(respones);
            }
            catch (Exception ex)
            {
                respones.Success = false;
                respones.Message = ex.Message;
                respones.Data = false;
                return BadRequest(respones);
            }
        }

    }
}

