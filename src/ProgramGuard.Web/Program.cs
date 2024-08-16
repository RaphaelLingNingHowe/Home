using Microsoft.AspNetCore.Authentication.Cookies;
using NLog.Extensions.Logging;
using ProgramGuard.Web.Pages;

// TODO 缺少 CSP

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();
// Set NLog
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
    loggingBuilder.AddNLog();
});
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddScoped<ChangePasswordModel>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
