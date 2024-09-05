using Employee_Role;
using ManagementAPI.Contract.Dtos.TasksDtos;
using ManagementAPI.Contract.Models;
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
        //public Task<List<GetTaskByIdDto>?> GetTaskById(int assigneToId);
        public Task<(int,List<GetTaskDto>?)> GetAllTasks(TaskPaginatedDto dto);
        public Task<GetTaskByIdDto?> GetTaskById(int id);
        public Task<(int, List<GetTaskDto>?)> GetParentChildTask(int projectId, int TaskId, bool children);
        public Task<int> AddTasks(AddTasksDto dtos);       
        public Task<int> UpdateTasks(UpdateTasksDto dto, int id);
        public Task<bool> DeleteTasks(int id );
        public  Task<bool> CheckManagerOfEmployee(int managerId, int? employeeId);
        public  Task<bool> CheckTeamMemberOfProject(int Member1Id, int? Member2Id, int ProjectId);
        public  Task<int> CheckTaskAccess(EmployeeRole Role, Tasks tasks, int AccessingId, Project project);







    }
}
