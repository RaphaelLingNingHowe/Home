using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.FileList;
using ProgramGuard.Enums;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    public class FileListModel : BasePageModel
    {
        public FileListModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_FILELIST))
            {
                await LogActionAsync(ACTION.ACCESS_FILELIST_PAGE, "嘗試進入無權限頁面");
                return RedirectToLoginPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync()
        {
            HttpResponseMessage response = await SendApiRequestAsync("/FileList", HttpMethod.Get);
            return await HandleResponseAsync<IEnumerable<GetFileListDto>>(response);
        }

        public async Task<IActionResult> OnPostAsync([FromBody] CreateFileListDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync("/FileList", HttpMethod.Post, dto);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnPatchAsync(int key, string values)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/FileList/{key}", HttpMethod.Patch, values);
            return await HandleResponseAsync(response);
        }

        public async Task<IActionResult> OnDeleteAsync(int key)
        {
            HttpResponseMessage response = await SendApiRequestAsync($"/FileList/{key}", HttpMethod.Delete);
            return await HandleResponseAsync(response);
        }
    }
}
