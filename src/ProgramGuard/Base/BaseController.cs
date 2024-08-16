using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Enums;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;
using System.Security.Claims;

namespace ProgramGuard.Base
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly ProgramGuardContext _context;

        protected VISIBLE_PRIVILEGE _visiblePrivilege;
        protected OPERATE_PRIVILEGE _operatePrivilege;

        public BaseController(ProgramGuardContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            if (principal.FindFirstValue(ClaimTypes.NameIdentifier) is string account && !string.IsNullOrEmpty(account))
            {
                return await _context.Users.SingleOrDefaultAsync(u => u.Account == account);
            }

            return null;
        }

        protected async Task LogActionAsync(User user, ACTION action, string comment = "")
        {
            OperateLog operateLog = new()
            {
                User = user,
                Action = action,
                Comment = comment,
            };

            await _context.AddAsync(operateLog);
            await _context.SaveChangesAsync();
        }

        protected async Task LogActionAsync(ACTION action, string comment = "")
        {
            if (await GetUserAsync(User) is not User user)
            {
                _logger.LogError("記錄{action}操作失敗，因為找不到帳號，備註：{comment}", action, comment);
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
