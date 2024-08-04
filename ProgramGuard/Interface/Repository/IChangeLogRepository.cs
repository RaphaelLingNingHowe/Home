using ProgramGuard.Models;

namespace ProgramGuard.Interface.Repository
{
    public interface IChangeLogRepository : IRepository<ChangeLog>
    {
        Task<IEnumerable<ChangeLog>> GetByQueryAsync(DateTime? startTime, DateTime? endTime, string fileName, bool? isConfirmed);
        Task<string?> GetLatestSha512Async(string filePath);
    }
}
