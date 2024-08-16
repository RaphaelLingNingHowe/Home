using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class IndexModel : BasePageModel
    {
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/FileList");
            }
            else
            {
                return RedirectToLoginPage();
            }
        }
    }
}
