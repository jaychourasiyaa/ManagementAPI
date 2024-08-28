using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksAPI;

namespace ManagementAPI.Contract.Interfaces
{
    public interface ITasksServices
    {
        public Task<(int,List<GetTaskDto?>)> GetAllTasks(EmployeeRole Role , int assignedTo, TaskPaginatedDto dto);
        public Task<int> AddTasks(AddTasksDtos dtos, int assignedBy);       
        public Task<int> UpdateTasks(UpdateTasksDto dto, int id, int AccessingId, EmployeeRole Role);
        public Task<List<GetTaskByIdDto>?> GetTaskById(int assigneToId);

        public Task<bool> DeleteTasks(int id ,int AccessingId, EmployeeRole Role);

        public Task<(int, List<GetTaskDto>?)> getTasks(int  projectId, int TaskId , bool children);






    }
}
