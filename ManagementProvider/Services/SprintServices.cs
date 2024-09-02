using ManagementAPI.Contract.Dtos.SprintDtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Contract.Models;
using ManagementAPI.Provider.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class SprintServices : ISprintServices
    {
        private readonly dbContext _dbContext;
        public SprintServices(dbContext db)
        {
            this._dbContext = db;
        }
        public async Task<int> AddSprint( AddSprintDto dto , int?toBeUpdated)
        {
            try
            {
                if( dto.startDate > dto.endDate )
                {
                    return -1;
                }
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
                if( project == null )
                {
                    return -2;
                }
                var sprint = new Sprint();
                if (toBeUpdated == null)
                {
                     sprint = new Sprint
                    {
                        Name = dto.Name,
                        StartDate = dto.startDate,
                        EndDate = dto.endDate,
                        ProjectId = project.Id,
                    };
                    await _dbContext.Sprints.AddAsync(sprint);
                }
                else
                {
                     sprint = await _dbContext.Sprints.FirstOrDefaultAsync(s => s.Id == toBeUpdated);
                    if( sprint == null )
                    {
                        return -3;
                    }
                    sprint.StartDate = dto.startDate;
                    sprint.EndDate = dto.endDate;
                    sprint.Name= dto.Name;
                    sprint.ProjectId = project.Id;
                }
                await _dbContext.SaveChangesAsync();
                return sprint.Id; 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<GetSprintDto>?> GetSprintUnderProject( int ? projectId)
        {
            try
            {
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if( project ==null )
                {
                    return null;
                }
                var sprints = await _dbContext.Sprints.Where( s=> s.ProjectId == projectId ).
                    Select( s => new GetSprintDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                    }).ToListAsync();
                return sprints;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
