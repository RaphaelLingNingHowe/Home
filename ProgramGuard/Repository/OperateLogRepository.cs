using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class OperateLogRepository : Repository<OperateLog>, IOperateLogRepository
    {
        public OperateLogRepository(ProgramGuardContext context) : base(context)
        {
        }
        public async Task<IEnumerable<OperateLog>> GetByQueryAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.OperateLogs
            .Where(o => o.Timestamp >= startTime && o.Timestamp <= endTime)
            .ToListAsync();
        }
    }
}
