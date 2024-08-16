using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class JwtCookieHelper
{
    public static async Task SignInJwtAsync(HttpContext httpContext, string token)
    {
        if (httpContext == null)
        {
            throw new InvalidOperationException("HTTP context is not available");
        }

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken != null)
        {
            var claims = jsonToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 设置身份验证 Cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = jsonToken.ValidTo
            };

            // 存储 JWT 令牌到 Cookie
            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken
                {
                    Name = "auth_token",
                    Value = token
                }
            });

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }
        else
        {
            throw new InvalidOperationException("Invalid JWT token");
        }
    }
}
