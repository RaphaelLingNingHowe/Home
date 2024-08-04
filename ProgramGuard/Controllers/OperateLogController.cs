using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;
using ProgramGuard.Repository;

namespace ProgramGuard.Controllers
{
    public class OperateLogController : BaseController
    {
        public OperateLogController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository) : base(context, logger, userRepository, operateLogRepository)
        {
        }
        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<ChangeLog>>> GetOperateLogsByQueryAsync([FromQuery] DateTime startTime, DateTime endTime)
        {
            if ((endTime - startTime).TotalDays > 7)
            {
                return BadRequest();
            }
            var operateLogs = await _operateLogRepository.GetByQueryAsync(startTime, endTime);
            return Ok(operateLogs);
        }
    }
}
