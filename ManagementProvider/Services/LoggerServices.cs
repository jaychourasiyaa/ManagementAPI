using ManagementAPI.Contract.Dtos;
using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class LoggerServices : ILoggerServices
    {
        private readonly dbContext _dbContext;
        public LoggerServices(dbContext db)
        {
            _dbContext = db;
        }
        public async Task<List<GetLogDto>?> GetLogById( int id )
        {
            try
            {
                var tasks = await _dbContext.Taasks.FirstOrDefaultAsync(t=> t.Id == id );
                if (tasks == null)
                {
                    return null;
                }
                var logs = await _dbContext.Logs.Include(t=> t.Taask).Where(t=> t.TaskId == id )
                    .Select( l=> new GetLogDto
                    {
                        Id = l.Id,
                        Name = l.Taask.Name,
                        Message = l.Message,
                        CreatedOn = l.dateTime,
                    }).ToListAsync();
                if( logs == null )
                {
                    return null;
                }
                return logs;
            }
            catch ( Exception ex )
            {
                throw ex;
            }
        }
    }
}
