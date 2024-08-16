using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Data.Config;
using ProgramGuard.Data.Dtos.ChangeLog;
using ProgramGuard.Enums;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;

namespace ProgramGuard.Controllers
{
    public class ChangeLogController : BaseController
    {
        private readonly QueryRangeSettings _queryRangeSettings;

        public ChangeLogController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<QueryRangeSettings> options) : base(context, logger)
        {
            _queryRangeSettings = options.Value;
        }

        [HttpGet("query")]
        public async Task<IActionResult> GetChangeLogsByQueryAsync([FromQuery] DateTime? startTime, DateTime? endTime, string? fileName, bool? unConfirmed)
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_CHANGELOG))
            {
                return Forbidden("權限不足，無法查看");
            }

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

                var timeRange = _queryRangeSettings.MaxRangeInDays;
                if ((endTime - startTime).Value.TotalDays > timeRange)
                {
                    return BadRequest($"時間範圍不能超過{timeRange}天");
                }
            }

            var query = _context.ChangeLogs
                .AsQueryable();

            string message = string.Empty;
            if (startTime.HasValue && endTime.HasValue)
            {
                query = query.Where(c => c.Timestamp >= startTime && c.Timestamp <= endTime);
                message += $"時間區間：{startTime}-{endTime}";
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                query = query.Where(c => c.FileList.Name.Contains(fileName));
                message += $" 檔案名稱-[{fileName}]";
            }

            if (unConfirmed == true)
            {
                query = query.Where(c => !c.IsConfirmed);
                message += " 僅顯示待審核記錄";
            }

            var result = await query.Select(c => new GetChangeLogDto
            {
                Id = c.Id,
                FileName = c.FileList.Name,
                Timestamp = c.Timestamp,
                UserName = c.User.Account,
                IsConfirmed = c.IsConfirmed,
                ConfirmedAt = c.ConfirmedAt,
                DigitalSignature = c.DigitalSignature
            }).ToListAsync();

            await LogActionAsync(ACTION.VIEW_CHANGELOG, message);

            if (!result.Any())
            {
                return NotFound("此區間沒有資料");
            }

            return Ok(result);
        }

        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> ConfirmChangeLogAsync(int id)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.CONFIRM_CHANGELOG))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.ChangeLogs.FindAsync(id) is not ChangeLog changeLog)
            {
                return NotFound($"找不到此異動記錄-[{id}]");
            }
            else if (changeLog.IsConfirmed)
            {
                return Forbidden("此異動記錄已審核");
            }

            if (await GetUserAsync(User) is not User user)
            {
                return NotFound("不明的使用者");
            }

            changeLog.UserId = user.Id;
            changeLog.IsConfirmed = true;
            changeLog.ConfirmedAt = DateTime.Now;
            await LogActionAsync(ACTION.CONFIRM_CHANGELOG, $"異動記錄-[{changeLog.Id}]");
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
