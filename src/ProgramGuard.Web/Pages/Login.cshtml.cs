using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgramGuard.Data.Dtos.Auth;
using ProgramGuard.Web.Base;

namespace ProgramGuard.Web.Pages
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel
    {
        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, logger, configuration, httpContextAccessor)
        {
        }

        public async Task<IActionResult> OnPostAsync([FromBody] LoginDto dto)
        {
            HttpResponseMessage response = await SendApiRequestAsync("/Auth/login", HttpMethod.Post, dto);
            if (!response.IsSuccessStatusCode)
            {
                return await HandleResponseAsync(response);
            }
            else
            {
                var statusCode = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(content);
                if (loginResponse == null)
                {
                    return StatusCode(500, new { message = "伺服器回應解析失敗" });
                }

                await JwtCookieHelper.SignInJwtAsync(HttpContext, loginResponse.Token);

                return new JsonResult(loginResponse.Message)
                {
                    StatusCode = statusCode
                };
            }
        }
    }
}


