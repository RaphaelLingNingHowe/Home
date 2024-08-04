using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ChangeLog;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    public class ChangeLogController : BaseController
    {
        private readonly IChangeLogRepository _changeLogRepository;
        public ChangeLogController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository, IChangeLogRepository changeLogRepository) : base(context, logger, userRepository, operateLogRepository)
        {
            _changeLogRepository = changeLogRepository;
        }

        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<GetChangeLogDto>>> GetChangeLogsByQueryAsync([FromQuery] DateTime? startTime, DateTime? endTime, string fileName, bool? isConfirmed)
        {
            if (!startTime.HasValue && !endTime.HasValue && string.IsNullOrEmpty(fileName) && !isConfirmed.HasValue)
            {
                return BadRequest("請提供至少一個查詢條件");
            }
            if (startTime.HasValue && endTime.HasValue && (endTime.Value - startTime.Value).TotalDays > 7)
            {
                return BadRequest("日期範圍不能超過 7 天");
            }
            var changeLogs = await _changeLogRepository.GetByQueryAsync(startTime, endTime, fileName, isConfirmed);
            var changeLogDtos = changeLogs.Select(c => new GetChangeLogDto
            {
                Id = c.Id,
                FileName = c.FileList.Name,
                Timestamp = c.Timestamp,
                ChangeType = c.ChangeType,
                ChangeDetail = c.ChangeDetail,
                UserName = c.User.Name,
                IsConfirmed = c.IsConfirmed,
                ConfirmedAt = c.ConfirmedAt,
                DigitalSIgnature = c.DigitalSIgnature
            });
            return Ok(changeLogs);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateChangeLogAsync(int id)
        {
            var changeLog = await _changeLogRepository.GetByIdAsync(id);
            if (changeLog == null)
            {
                return NotFound();
            }
            var currentUser = await _userRepository.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user could not be found for logging.");
                return Unauthorized();
            }
            changeLog.IsConfirmed = true;
            changeLog.UserId = currentUser.Id;
            await _changeLogRepository.UpdateAsync(changeLog);
            return NoContent();
        }
    }
}
