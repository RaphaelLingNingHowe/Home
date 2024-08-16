using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    public class PrivilegeRuleModel : BasePageModel
    {
        public PrivilegeRuleModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_PRIVILEGE_RULE))
            {
                await LogActionAsync(ACTION.ACCESS_PRIVILEGE_PAGE, "嘗試進入無權限頁面");
                return RedirectToLoginPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync()
        {
            HttpResponseMessage response = await SendApiRequestAsync("/PrivilegeRule", HttpMethod.Get);
            return await HandleResponseAsync<IEnumerable<GetPrivilegeRuleDto>>(response);
        }

        public async Task<IActionResult> OnGetPrivilegeAsync()
        {
            HttpResponseMessage response = await SendApiRequestAsync("/Privilege", HttpMethod.Get);
            return await HandleResponseAsync<PrivilegesDto>(response);
        }

        public async Task<IActionResult> OnPostAsync([FromBody] CreatePrivilegeRuleDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync("/PrivilegeRule", HttpMethod.Post, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchAsync(int key, [FromBody] UpdatePrivilegeRuleDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/PrivilegeRule/{key}/update-privilege", HttpMethod.Patch, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnDeleteAsync(int key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/PrivilegeRule/{key}", HttpMethod.Delete);
            return await HandleResponseAsync(response);
        }
    }
}
