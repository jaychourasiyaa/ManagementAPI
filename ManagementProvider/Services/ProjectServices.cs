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

namespace ManagementAPI.Provider.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly dbContext _dbContext;

        public ProjectServices(dbContext db)
        {
            _dbContext = db;

        }
        public async Task<int> AddProject(AddProjectDto addProjectDto)
        {
            try
            {
                var projectMaker = await _dbContext.Employees
                    .Where(e => e.Id == addProjectDto.CreatedBy)
                    .FirstOrDefaultAsync();

                if (projectMaker == null || !projectMaker.IsActive
                    || projectMaker.Role != EmployeeRole.SuperAdmin)
                {
                    return -1;
                }
                List<int> employeeIds = addProjectDto.Members;

                foreach (var employeeId in employeeIds)
                {
                    var employee = await _dbContext.Employees.Where(e => e.Id == employeeId).FirstOrDefaultAsync();
                    if (employee == null || !employee.IsActive /*|| employee.Role == EmployeeRole.SupderAdmin*/)
                    {
                        return -2;
                    }
                }
                if (Convert.ToInt32(addProjectDto.Status) < 1 || Convert.ToInt32(addProjectDto.Status) > 3)
                    return -3;

                var Project = new Project
                {
                    Name = addProjectDto.Name,
                    Description = addProjectDto.Description,
                    AssignedById = addProjectDto.CreatedBy,
                    Status = addProjectDto.Status

                };
                await _dbContext.AddAsync(Project);
                await _dbContext.SaveChangesAsync();
                foreach (var employeeId in employeeIds)
                {
                    var projectemployee = new ProjectEmployee { EmployeeID = employeeId, ProjectID = Project.Id };
                    _dbContext.ProjectEmployees.Add(projectemployee);
                }
                await _dbContext.SaveChangesAsync();
                return Project.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<GetProjectDetailsDto?> GetById(int id)
        {
            try
            {
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
                    .Select(e => new NameAndIdEmployeeDto
                    {
                        Name = e.Employee.Name,
                        Id = e.Employee.Id,
                    }).ToList();
                var projectDetails = new GetProjectDetailsDto
                {

                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.ProjectMaker.Name,
                    CreatedOn = project.CreatedOn,
                    Status = project.Status,
                    ProjectEmployee = project.ProjectEmployee
                    .Select(e => new NameAndIdEmployeeDto {Name = e.Employee.Name, Id = e.Employee.Id}).ToList(),
                };

                return projectDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<GetProjectDetailsDto>?> GetAllProject()
        {
            try
            {
                
                var projects = await _dbContext.Projects.Select(p => new GetProjectDetailsDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    CreatedBy = p.ProjectMaker.Name,
                    CreatedOn = p.CreatedOn,
                    Status = p.Status,
                    ProjectEmployee =p.ProjectEmployee
                    .Select(e => new NameAndIdEmployeeDto
                    {
                        Name = e.Employee.Name,
                        Id = e.Employee.Id,
                    }).ToList()

                }).ToListAsync();
                
                if (projects == null) return null;
                return projects;
            }
            catch ( Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        }
    }
