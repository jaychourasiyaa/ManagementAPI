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
        public Task<List<GetTaskDto?>> GetAllTasks(EmployeeRole Role , int assignedTo);
        public Task<int> AddTasks(AddTasksDtos dtos, int assignedBy);       
        public Task<int> UpdateTasks(int status, int id, int AccessingId);
        public Task<List<TaskDtos>?> GetTaskById(int assigneToId);

        public Task<bool> DeleteTasks(int id ,int AccessingId);
       





    }
}
