using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using ProgramGuard.Base;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ChangeLog;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProgramGuard.Controllers
{
    public class ChangeLogController : BaseController
    {
        private readonly IOptions<TimeRangeSettings> _options;
        public ChangeLogController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<TimeRangeSettings> options) : base(context, logger)
        {
            _options = options;
        }
        [HttpGet("query")]
        public async Task<IActionResult> GetChangeLogsByQueryAsync([FromQuery] DateTime? startTime, DateTime? endTime, string fileName, bool? unConfirmed)
        {
            if (!startTime.HasValue && !endTime.HasValue && string.IsNullOrEmpty(fileName) && !unConfirmed.HasValue)
            {
                return BadRequest("請提供至少一個查詢條件");
            }
            if (startTime.HasValue && endTime.HasValue)
            {
                if (!startTime.HasValue && !endTime.HasValue)
                {
                    return BadRequest("請提供查詢時間");
                }
                if (endTime < startTime)
                {
                    return BadRequest("結束時間不能早於開始時間");
                }
                var timeRange = _options.Value.MaxRangeInDays;
                if ((endTime - startTime).Value.TotalDays > timeRange)
                {
                    return BadRequest($"時間範圍不能超過{timeRange}天");
                }
            }
            var query = _context.ChangeLogs
                .AsQueryable();
            if (startTime.HasValue && endTime.HasValue)
            {
                query = query.Where(c => c.Timestamp >= startTime && c.Timestamp <= endTime);
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                query = query.Where(c => c.FileList.Name.Contains(fileName));
            }
            if (unConfirmed == true)
            {
                query = query.Where(c => !c.IsConfirmed);
            }
            var result = await query.Select(c => new GetChangeLogDto
            {
                Id = c.Id,
                FileName = c.FileList.Name,
                Timestamp = c.Timestamp,
                ChangeType = EnumHelper.GetEnumDescription(c.ChangeType),
                ChangeDetail = c.ChangeDetail,
                UserName = c.User.Name,
                IsConfirmed = c.IsConfirmed,
                ConfirmedAt = c.ConfirmedAt,
                DigitalSignature = c.DigitalSignature
            }).ToListAsync();
            return Ok(result);
        }
    }
}
