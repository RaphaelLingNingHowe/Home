using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data.Dtos.User;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    public class ChangePasswordModel : BasePageModel
    {
        public ChangePasswordModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string key, [FromBody] ChangePasswordDto dto)
        {
            var response = await SendApiRequestAsync($"/User/{key}/change-password", HttpMethod.Post, dto);
            return await HandleResponseAsync(response);
        }
    }
}
