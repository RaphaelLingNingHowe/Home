using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    public class UserModel : BasePageModel
    {
        public ResetPasswordDto ResetPasswordDto { get; set; } = new ResetPasswordDto();

        public UserModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_USER))
            {
                await LogActionAsync(ACTION.ACCESS_USER_PAGE, "嘗試進入無權限頁面");
                return RedirectToLoginPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync()
        {
            HttpResponseMessage response = await SendApiRequestAsync("/User", HttpMethod.Get);
            return await HandleResponseAsync<IEnumerable<GetUserDto>>(response);
        }

        public async Task<IActionResult> OnPostAsync([FromBody] CreateUserDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync("/User", HttpMethod.Post, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string key, [FromBody] ResetPasswordDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/reset-password", HttpMethod.Post, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchNameAsync(string key, [FromBody] UpdateUserNameDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/name", HttpMethod.Patch, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchPrivilegeAsync(string key, [FromBody] UpdateUserPrivilegeDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/privilege", HttpMethod.Patch, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchEnableAsync(string key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/enable", HttpMethod.Patch);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchDisableAsync(string key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/disable", HttpMethod.Patch);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchUnlockAsync(string key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}/unlock", HttpMethod.Patch);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnDeleteAsync(string key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/User/{key}", HttpMethod.Delete);
            return await HandleResponseAsync(response);
        }

        //public async Task<IActionResult> OnGetPrivilegeAsync()
        //{
        //    HttpResponseMessage response = await SendApiRequest("/PrivilegeRule/Privilege", HttpMethod.Get);
        //    return await HandleResponseAsync<IEnumerable<PrivilegesDto>>(response);
        //}
    }
}
