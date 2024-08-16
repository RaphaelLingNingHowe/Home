using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.ChangeLog;
using ProgramGuard.Enums;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    public class ChangeLogModel : BasePageModel
    {
        public ChangeLogModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_CHANGELOG))
            {
                await LogActionAsync(ACTION.ACCESS_CHANGELOG_PAGE, "嘗試進入無權限頁面");
                return RedirectToLoginPage();
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync(string? startTime, string? endTime, string? fileName, bool? unConfirmed)
        {
            var response = await SendApiRequestAsync($"/ChangeLog/query?startTime={startTime}&endTime={endTime}&fileName={fileName}&unConfirmed={unConfirmed}", HttpMethod.Get);
            return await HandleResponseAsync<IEnumerable<GetChangeLogDto>>(response);
        }

        public async Task<IActionResult> OnPatchConfirmAsync(int key)
        {
            var response = await SendApiRequestAsync($"/ChangeLog/{key}/confirm", HttpMethod.Patch);
            return await HandleResponseAsync(response);
        }
    }
}
