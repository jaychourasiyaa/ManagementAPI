using Employee_Role;
using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAPI.Contract.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ManagementAPI.Contract.Responses;
using System.Runtime.InteropServices;
using ManagementAPIEmployee;
using Microsoft.Identity.Client;
using System.ComponentModel.Design;
using ManagementAPI.Contract.Dtos.EmployeeDtos;
using ManagementAPI.Contract.Enums;
using System.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;


namespace ManagementAPI.Provider.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly dbContext _dbContext;

        public ProjectServices(dbContext db)
        {
            _dbContext = db;

        }
        public IQueryable<GetProjectDetailsDto>? ApplyFiltering(IQueryable<GetProjectDetailsDto>? projects, string? filterOn, string? filterQuery , DateTime? startDate, DateTime? endDate)
        {
            if( startDate != null && endDate != null )
            {
                projects = projects.Where( p=> p.CreatedOn >= startDate && p.CreatedOn <= endDate );
            }
            if (string.IsNullOrEmpty(filterQuery) == false)
            {
                // filer according to name or subpart of name
                if (filterOn.Equals("", StringComparison.OrdinalIgnoreCase) && filterQuery != "")
                {
                    projects = projects.Where(e => e.Name.Contains(filterQuery)
                    || e.CreatedBy.Contains(filterQuery));
                }

                // filter according to department or subpart of department name
                /*else if (filterOn.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
                {
                    projects = projects.Where(e => e.CreatedBy.Contains(filterQuery));
                }*/
                else if (filterOn.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    // Checking value of filterQuery is number or not
                    if (int.TryParse(filterQuery, out var statusId))
                    {
                        // if number then check for valid number from 0-2
                        if (Enum.IsDefined(typeof(ProjectStatus), statusId))
                        {
                            var status = (ProjectStatus)statusId;
                            projects = projects.Where(e => e.Status == status);
                        }
                        else
                        {
                            throw new Exception("Invalid Status ID specified.");
                        }
                    }
                    else if( filterQuery != "")
                    {
                        // Normalize filterQuery to match enum names case insensitively
                        var normalizedFilterQuery = filterQuery.Trim().ToLowerInvariant();

                        var matchingStatus = Enum.GetValues(typeof(ProjectStatus))
                            .Cast<ProjectStatus>()
                            .FirstOrDefault(role => role.ToString().ToLowerInvariant() == normalizedFilterQuery);

                       /* if (matchingStatus != default(ProjectStatus))
                        {*/
                            projects = projects.Where(e => e.Status == matchingStatus);
                        /*}
                        else
                        {
                            throw new Exception("Invalid Status name specified.");
                        }*/
                    }
                }


            }
            return projects;
        }

        public IQueryable<GetProjectDetailsDto>? ApplySorting(IQueryable<GetProjectDetailsDto>? employee, string? SortBy,
            bool IsAscending)
        {
            if (string.IsNullOrEmpty(SortBy) == false)
            {
                if (SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {

                    employee = IsAscending ? employee.OrderBy(e => e.Name) : employee.OrderByDescending(e => e.Name);

                }
                if (SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.CreatedOn) :
                    employee.OrderByDescending(e => e.CreatedOn);

                }
                if (SortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    employee = IsAscending ? employee.OrderBy(e => e.Id) :
                        employee.OrderByDescending(e => e.Id);
                }
               
                
               
            }
            return employee;
        }
        public bool checkAlreadyExists(int employeeId, int projectId)
        {
            var employee = _dbContext.ProjectEmployees.FirstOrDefault(e => e.ProjectID == projectId
             && e.EmployeeID == employeeId);
            if (employee == null)
            {
                return false;
            }
            return true;
        }
        public bool CheckEmployeeId(int employeeId)
        {
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null || !employee.IsActive)
            {
                return false;
            }
            return true;
        }
        public async Task<int> AddProject(AddProjectDto addProjectDto , int createdBy)
        {
            try
            {
                // checking if the person making project exists in database or not
                var projectMaker = await _dbContext.Employees
                    .Where(e => e.Id == createdBy)
                    .FirstOrDefaultAsync();

                // if not exists or soft deleted or not a superadmin returing -1 to controller
                if (projectMaker == null || !projectMaker.IsActive
                    || projectMaker.Role != EmployeeRole.SuperAdmin)
                {
                    return -1;
                }

                // if project status is not valid status != pending , running , completed
                if (Convert.ToInt32(addProjectDto.Status) < 0 || Convert.ToInt32(addProjectDto.Status) > 2)
                    return -3;

                // by default the  empty list contains 0 removing that , 
                // considering null if user has passed 0 in list
                List<int> employeeIds = addProjectDto.Members.Where(p => p != 0).Distinct().ToList();

                
               // cheking if the memeber id to be assigned to the project is a valid id or not
                foreach (var employeeId in employeeIds)
                {
                    var employee = await _dbContext.Employees.Where(e => e.Id == employeeId).FirstOrDefaultAsync();

                    // if not valid or soft deleted returning -2
                    if (employee == null || !employee.IsActive )
                    {
                        return -2;
                    }
                    
                }
     
                
                

                var Project = new Project
                {
                    Name = addProjectDto.Name,
                    Description = addProjectDto.Description,
                    AssignedById = createdBy,
                    Status = addProjectDto.Status,
                    CreatedBy = createdBy

                };
                await _dbContext.AddAsync(Project);
                await _dbContext.SaveChangesAsync();
                if (employeeIds.Count > 0)
                {
                    foreach (var employeeId in employeeIds)
                    {
                        var projectemployee = new ProjectEmployee { EmployeeID = employeeId, ProjectID = Project.Id };
                        _dbContext.ProjectEmployees.Add(projectemployee);
                    }
                }
                await _dbContext.SaveChangesAsync();
                return Project.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ProjectDetailsByIdDto?> GetById(int id)
        {
            try
            {
                // getting details of project accordint to project id
                var project = await _dbContext.Projects
                    .Include(p => p.ProjectMaker)
                    .Include(e => e.ProjectEmployee)
                    .ThenInclude(pe => pe.Employee)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return null;
                }

                var projectemployees = project.ProjectEmployee
                    .Select(e => new IdandNameDto
                    {
                        Name = e.Employee.Name,
                        Id = e.Employee.Id,
                    }).ToList();
                var projectDetails = new ProjectDetailsByIdDto
                {
                  

                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.ProjectMaker.Name,
                 
                    CreatedOn = project.CreatedOn,
                    Status = project.Status,
                    ProjectEmployee = project.ProjectEmployee
                    .Select(e => new IdandNameDto { Name = e.Employee.Name, Id = e.Employee.Id}).ToList(),
                };

                return projectDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<(int,List<GetProjectDetailsDto>?)> GetAllProject(int employeeId, PaginatedGetDto dto)
        {

            try
            {
                string? filterOn = "";
                if (dto.filterOn.Equals("Status" , StringComparison.OrdinalIgnoreCase))
                {
                    filterOn = "Status";
                }
                string? filterQuery = dto.filterQuery;
                string? SortBy = dto.SortBy;
                bool IsAscending = dto.IsAscending;
                int pageNumber = dto.pageNumber <= 0 ? 1 : dto.pageNumber;
                int pageSize = dto.pageSize <= 0 ? 10 : dto.pageSize;
                string ? additonalSearch = dto.additionalSearch;
                DateTime? startDate = dto.startDate;
                DateTime? endDate = dto.endDate;

                // getting project of an employee role wise
                var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
                int count = 0;

                // if employee is null or soft deleted returning false
                /*if (employee == null || employee.IsActive == false)
                {
                    return (null,null);
                }*/
                IQueryable<GetProjectDetailsDto>? projects;

                // if employee role is super admin giving all project to them
                if (employee != null && employee.Role == EmployeeRole.SuperAdmin)
                {
                    projects = _dbContext.Projects.Include(p => p.ProjectEmployee).ThenInclude(pe => pe.Employee).Select(p => new GetProjectDetailsDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CreatedBy = p.ProjectMaker.Name,
                        CreatedOn = p.CreatedOn,
                     
                        Status = p.Status,
                        ProjectEmployee = p.ProjectEmployee
                            .Where(p => p.IsActive == true)
                            .Select(e => new IdandNameDto
                            {
                                Name = e.Employee.Name,
                                Id = e.Employee.Id,

                            }).ToList()

                    }).AsQueryable();
                    count = await projects.CountAsync();
                   /* return projects.ToList();*/
                }
                else
                {
                    // if employee role is not superadmin
                    var employeeUnderManager = _dbContext.Employees
                        .Where(e => e.AdminId == employee.Id)
                        .Select(e => e.Id)
                        .ToList();
                    employeeUnderManager.Add(employeeId);

                    // fetching their project and their downline project if employee is manager
                     projects = _dbContext.Projects
                                  .Include(p => p.ProjectEmployee)
                                  .ThenInclude(pe => pe.Employee)
                                  .Distinct()
                                  .Where(p => p.ProjectEmployee.Any(pe => employeeUnderManager.Contains(pe.EmployeeID)))
                                   .Select(p => new GetProjectDetailsDto
                                   {

                                       Id = p.Id,
                                       Name = p.Name,
                                       Description = p.Description,
                                       CreatedBy = p.ProjectMaker.Name,
                                       CreatedOn = p.CreatedOn,
                                      
                                       Status = p.Status,
                                       ProjectEmployee = p.ProjectEmployee
                                       .Where(p => p.IsActive == true)
                                       .Select(pe => new IdandNameDto
                                       {
                                           Name = pe.Employee.Name,
                                           Id = pe.Employee.Id,

                                       }).ToList()

                                   }).AsQueryable();

                    count= await projects.CountAsync();
                    
                }
                /*if (projects == null) 
                    return null;*/
                
                projects = ApplyFiltering(projects,filterOn,filterQuery , startDate, endDate);
                projects = ApplySorting(projects, SortBy, IsAscending);
                if( additonalSearch != "")
                {
                    projects = projects.Where(e => e.Name.Contains(additonalSearch)
                   || e.CreatedBy.Contains(additonalSearch));
                }
                var skipResult = (pageNumber - 1) * pageSize; 
                return  (count,await projects.Skip(skipResult).Take(pageSize).ToListAsync());
            }
            catch ( Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int?> UpdateProject(int projectId, AddProjectDto dto, int updatedBy)
        {
            try
            {
                // checking if provided project id is valid or not
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                
                // if project not found 
                if (project == null) return -1;

                // handling not initialized error object refrence not set to and object instance
                if (project.ProjectEmployee == null)
                {
                    project.ProjectEmployee = new List<ProjectEmployee>();
                }

                // filtering the default values 
                List<int> employeeIds = dto.Members.Where(m => m != 0).ToList();
                foreach (var employeeId in employeeIds)
                {
                    // if employee id is valid
                    bool employeeIdExists = CheckEmployeeId(employeeId);
                    if (employeeIdExists)
                    {
                        // if employee already exists in project not adding hiim
                        bool checkAlreadyExist = checkAlreadyExists(employeeId, projectId);
                        if (!checkAlreadyExist)
                        {
                            var projectemployee = new ProjectEmployee
                            {
                                EmployeeID = employeeId,
                                ProjectID = projectId
                            };
                             project.ProjectEmployee.Add(projectemployee);
                        }
                    }
                    else return -2;
                }
                project.Name = dto.Name;
                project.Description = dto.Description;
                project.UpdatedOn = DateTime.Now;
                project.UpdatedBy = updatedBy;
                project.Status = dto.Status;
                await _dbContext.SaveChangesAsync();
                return project.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

    }
        public async  Task<bool> DeleteMember( int employeeId , int  projectId )
        {
            var employee = await _dbContext.ProjectEmployees.Where( p=> p.EmployeeID == employeeId 
                            && p.ProjectID == projectId && p.IsActive).FirstOrDefaultAsync();
            if (employee == null) return false;
            employee.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<int?> GetCountStatusWise(int status)
        {
            int? count = 0;
            if (status == 0)
            {
                count = await _dbContext.Projects
                              .Where(p => p.Status == ProjectStatus.Created )
                              .CountAsync();
            }
            else if (status == 1)
            {
                count = await _dbContext.Projects
                             .Where(p => p.Status == ProjectStatus.Running)
                             .CountAsync();
            }
            else if (status == 2)
            {
                count = await _dbContext.Projects
                             .Where(p=> p.Status == ProjectStatus.Completed)
                             .CountAsync();
            }
            else
            {
                return null;
            }
            return count;
        }
        public async Task<(int, List<GetTaskDto>?)> getTasks(int? projectId, int? parentId)
        {
            List<GetTaskDto>? tasks = new List<GetTaskDto>();
            int count = 0;
            if (parentId == null && projectId != null)
            {
                tasks = await _dbContext.Taasks.Where(t => t.ProjectId == projectId && t.ParentId == null).
                    Select(t => new GetTaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        AssignedById = t.AssignedById,
                        AssignedToId = t.AssignedToId,
                        Assigned_From = t.AssignedBy.Name,
                        Assigned_To = t.AssignedTo.Name,
                        CreatedOn = t.CreatedOn,
                        ParentId = t.ParentId,
                        ProjectId = t.ProjectId,
                        Status = t.Status
                    }).ToListAsync();
                count = tasks.Count;
            }
            else if (parentId != null && projectId != null)
            {
                tasks = await _dbContext.Taasks.Where(t => t.ProjectId == projectId && t.ParentId == parentId).
                    Select(t => new GetTaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        AssignedById = t.AssignedById,
                        AssignedToId = t.AssignedToId,
                        Assigned_From = t.AssignedBy.Name,
                        Assigned_To = t.AssignedTo.Name,
                        CreatedOn = t.CreatedOn,
                        ParentId = t.ParentId,
                        ProjectId = t.ProjectId,
                        Status = t.Status
                    }).ToListAsync();
                count = tasks.Count;
            }
            
            return ( count, tasks );
            
        }
    }
   
    }
