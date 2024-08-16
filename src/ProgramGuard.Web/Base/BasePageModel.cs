using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ProgramGuard.Data.Dtos.ChangeLog;
using ProgramGuard.Enums;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
namespace ProgramGuard.Web.Base
{
    [Authorize]
    public class BasePageModel : PageModel
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<BasePageModel> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _httpContext;
        protected VISIBLE_PRIVILEGE? _visiblePrivilege;
        protected OPERATE_PRIVILEGE? _operatePrivilege;

        public BasePageModel(IHttpClientFactory httpClientFactory, ILogger<BasePageModel> logger, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _httpContext = httpContext;
        }

        protected async Task<HttpResponseMessage> SendApiRequestAsync(string endpoint, HttpMethod method, object? data = null)
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration.GetValue<string>("ApiEndpoint:BaseUrl");
            var request = new HttpRequestMessage(method, $"{baseUrl}{endpoint}");
            var token = _httpContext.HttpContext?.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "auth_token").Result;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            if (data is string jsonData)
            {
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }
            else
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            }

            return await client.SendAsync(request);
        }

        protected async Task<JsonResult> HandleResponseAsync(HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;
            var content = await response.Content.ReadAsStringAsync();
            var result = new
            {
                message = !string.IsNullOrEmpty(content) ? content : null
            };
            return new JsonResult(result)
            {
                StatusCode = statusCode
            };
        }

        protected async Task<JsonResult> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;
            object? result;
            if (response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                result = new { message = content };
            }
            return new JsonResult(result)
            {
                StatusCode = statusCode
            };
        }

        protected async Task LogActionAsync(ACTION action, string comment = "")
        {
            LogActionDto dto = new()
            {
                Action = action,
                Comment = comment
            };
            await SendApiRequestAsync("/OperateLog", HttpMethod.Post, dto);
        }
        protected IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("/Login");
        }

        protected VISIBLE_PRIVILEGE VisiblePrivilege
        {
            get
            {
                if (_visiblePrivilege == null)
                {
                    if (User.Claims.FirstOrDefault(c => c.Type == "visiblePrivilege") is Claim rawClaims && uint.TryParse(rawClaims.Value, out uint privilege))
                    {
                        _visiblePrivilege = (VISIBLE_PRIVILEGE)privilege;
                    }
                    else
                    {
                        _visiblePrivilege = VISIBLE_PRIVILEGE.UNKNOWN;
                    }
                }

                return (VISIBLE_PRIVILEGE)_visiblePrivilege;
            }
        }

        protected OPERATE_PRIVILEGE OperatePrivilege
        {
            get
            {
                if (_operatePrivilege == null)
                {
                    if (User.Claims.FirstOrDefault(c => c.Type == "operatePrivilege") is Claim rawClaims && uint.TryParse(rawClaims.Value, out uint privilege))
                    {
                        _operatePrivilege = (OPERATE_PRIVILEGE)privilege;
                    }
                    else
                    {
                        _operatePrivilege = OPERATE_PRIVILEGE.UNKNOWN;
                    }
                }

                return (OPERATE_PRIVILEGE)_operatePrivilege;
            }
        }
    }
}
