using ProgramGuard.Models;

namespace ProgramGuard.Interface.Repository
{
    public interface IOperateLogRepository : IRepository<OperateLog>
    {
        Task<IEnumerable<OperateLog>> GetByQueryAsync(DateTime startTime, DateTime endTime);
    }
}
