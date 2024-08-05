using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ChangeLog;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Dtos.OperateLog;
using ProgramGuard.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProgramGuard.Controllers
{
    public class OperateLogController : BaseController
    {
        private readonly IOptions<TimeRangeSettings> _options;
        public OperateLogController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<TimeRangeSettings> options) : base(context, logger)
        {
            _options = options;
        }

        [HttpGet("query")]
        public async Task<IActionResult> GetOperateLogsByQueryAsync([FromQuery] DateTime startTime, DateTime endTime)
        {
            var timeRange = _options.Value.MaxRangeInDays;
            if ((endTime - startTime).TotalDays > timeRange)
            {
                return BadRequest($"時間範圍不能超過{timeRange}天");
            }
            IEnumerable<GetOperateLogDto> result = await _context.OperateLogs
                        .Where(o => o.Timestamp >= startTime && o.Timestamp <= endTime)
                        .OrderByDescending(o => o.Timestamp)
                        .Select(o => new GetOperateLogDto()
                        {
                            Id = o.Id,
                            Account = o.User.Account,
                            Comment = o.Comment,
                            Timestamp = o.Timestamp
                        })
                        .ToListAsync();
            return Ok(result);
        }
    }
}
