using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class ChangeLogRepository : Repository<ChangeLog>, IChangeLogRepository
    {
        public ChangeLogRepository(ProgramGuardContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChangeLog>> GetByQueryAsync(DateTime? startTime, DateTime? endTime, string fileName, bool? isConfirmed)
        {
            var query = _context.ChangeLogs.AsQueryable();
            if (!startTime.HasValue && !endTime.HasValue && string.IsNullOrEmpty(fileName) && !isConfirmed.HasValue)
            {
                throw new ArgumentException("At least one query parameter must be provided.");
            }
            if (startTime.HasValue)
            {
                query = query.Where(c => c.Timestamp >= startTime.Value);
            }
            if (endTime.HasValue)
            {
                query = query.Where(c => c.Timestamp <= endTime.Value);
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                query = query.Where(c => _context.FileLists.Any(f => f.Id == c.FileListId && f.Name.Contains(fileName)));
            }
            if (isConfirmed.HasValue)
            {
                query = query.Where(c => c.IsConfirmed == isConfirmed.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<string?> GetLatestSha512Async(string filePath)
        {
            return await _context.ChangeLogs
                .Where(c => c.FileList != null && c.FileList.Path == filePath)
                .OrderByDescending(c => c.Timestamp)
                .Select(c => c.Sha512)
                .FirstOrDefaultAsync();
        }
    }
}
