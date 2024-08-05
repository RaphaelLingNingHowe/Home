using ProgramGuard.Base;
using ProgramGuard.Data;

namespace ProgramGuard.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(ProgramGuardContext context, ILogger<BaseController> logger) : base(context, logger)
        {
        }
    }
}
