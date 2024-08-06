using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Enums;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Base
{
    [Route("[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        // 與 protected VISIBLE_PRIVILEGE _visiblePrivilege; 等效, 因為 VISIBLE_PRIVILEGE.UNKNOWN 為預設值
        protected VISIBLE_PRIVILEGE _visiblePrivilege = VISIBLE_PRIVILEGE.UNKNOWN;
        protected OPERATE_PRIVILEGE _operatePrivilege = OPERATE_PRIVILEGE.UNKNOWN;
        protected readonly ProgramGuardContext _context;
        protected readonly ILogger<BaseController> _logger;
        public BaseController(ProgramGuardContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            var account = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return account != null ? await _context.Users.FirstOrDefaultAsync(u => u.Account == account) : null;
        }

        protected async Task LogActionAsync(ACTION action, string comment = "")
        {
            if (await GetUserAsync(User) is not User user)
            {
                _logger.LogError($"記錄'{action}'操作失敗，因為找不到帳號，備註：'{comment}'");
                return;
            }

            OperateLog operateLog = new()
            {
                User = user,
                Action = action,
                Comment = comment,
            };
            await _context.AddAsync(operateLog);
            await _context.SaveChangesAsync();
        }

        protected VISIBLE_PRIVILEGE VisiblePrivilege
        {
            get
            {
                if (_visiblePrivilege == VISIBLE_PRIVILEGE.UNKNOWN)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "visiblePrivilege");
                    if (claim != null && uint.TryParse(claim.Value, out uint privilege))
                    {
                        _visiblePrivilege = (VISIBLE_PRIVILEGE)privilege;
                    }
                }

                return _visiblePrivilege;
            }
        }

        protected OPERATE_PRIVILEGE OperatePrivilege
        {
            get
            {
                if (_operatePrivilege == OPERATE_PRIVILEGE.UNKNOWN)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "operatePrivilege");
                    if (claim != null && uint.TryParse(claim.Value, out uint privilege))
                    {
                        _operatePrivilege = (OPERATE_PRIVILEGE)privilege;
                    }
                }

                return _operatePrivilege;
            }
        }

        [NonAction]
        public ObjectResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }

        [NonAction]
        public ObjectResult Created(object value)
        {
            return StatusCode(201, value);
        }

        [NonAction]
        public ObjectResult ServerError(object value)
        {
            return StatusCode(500, value);
        }

        [NonAction]
        public new ObjectResult BadRequest(object value)
        {
            return StatusCode(400, value);
        }
    }
}
