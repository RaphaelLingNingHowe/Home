using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.OperateLog;
using ProgramGuard.Enums;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages.Account
{
    public class OperateLogModel : BasePageModel
    {
        public OperateLogModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_OPERATELOG))
            {
                await LogActionAsync(ACTION.ACCESS_OPERATELOG_PAGE, "嘗試進入無權限頁面");
                return RedirectToLoginPage();
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync(string startTime, string endTime)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/OperateLog/query?startTime={startTime}&endTime={endTime}", HttpMethod.Get);
            return await HandleResponseAsync<IEnumerable<GetOperateLogDto>>(response);
        }
    }
}
