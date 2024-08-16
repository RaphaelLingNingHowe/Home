using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class LogoutModel : BasePageModel
    {
        public LogoutModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await SendApiRequestAsync("/Auth/logout", HttpMethod.Post);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("auth_token");

            return RedirectToLoginPage();
        }
    }
}
