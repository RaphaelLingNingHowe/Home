using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Enums;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;

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
        protected readonly IUserRepository _userRepository;
        protected readonly IOperateLogRepository _operateLogRepository;

        public BaseController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository)
        {
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
            _operateLogRepository = operateLogRepository;
        }

        protected async Task LogActionAsync(ACTION action, string comment = "")
        {
            var currentUser = await _userRepository.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user could not be found for logging.");
                return;
            }
            OperateLog operateLog = new()
            {
                User = currentUser,
                Action = action,
                Comment = comment,
            };
            await _operateLogRepository.AddAsync(operateLog);
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
