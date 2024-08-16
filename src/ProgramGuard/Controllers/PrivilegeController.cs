using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Enums;
using ProgramGuard.Repository.Data;
using ProgramGuard.Services;

namespace ProgramGuard.Controllers
{
    public class PrivilegeController : BaseController
    {
        public PrivilegeController(ProgramGuardContext context, ILogger<BaseController> logger) : base(context, logger)
        {
        }

        [HttpGet]
        public IActionResult GetAllPrivileges()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_PRIVILEGE_RULE) && !VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_USER))
            {
                return Forbidden("權限不足，無法查看");
            }

            return Ok(Privilege.Privileges);
        }
    }
}
