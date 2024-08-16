using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Data.Config;
using ProgramGuard.Data.Dtos.ChangeLog;
using ProgramGuard.Data.Dtos.OperateLog;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;

namespace ProgramGuard.Controllers
{
    public class OperateLogController : BaseController
    {
        private readonly QueryRangeSettings _options;

        public OperateLogController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<QueryRangeSettings> options) : base(context, logger)
        {
            _options = options.Value;
        }

        [HttpGet("query")]
        public async Task<IActionResult> GetOperateLogsByQueryAsync([FromQuery] DateTime startTime, DateTime endTime)
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_OPERATELOG))
            {
                return Forbidden("權限不足，無法查看");
            }

            var timeRange = _options.MaxRangeInDays;
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
                            Action = o.Action.GetEnumDescription(),
                            Comment = o.Comment,
                            Timestamp = o.Timestamp
                        })
                        .ToListAsync();

            await LogActionAsync(ACTION.VIEW_OPERATELOG, $"時間區間：{startTime}-{endTime}");

            if (!result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> LogActionAsync(LogActionDto dto)
        {
            await LogActionAsync(dto.Action);
            return Created();
        }
    }
}
