using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using ProgramGuard.Data.Config;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;
using ProgramGuard.Services;
using ProgramGuard.Workers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();
// Set NLog
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
    loggingBuilder.AddNLog();
});
// Add services to the container.
builder.Services.AddDbContextFactory<ProgramGuardContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
    var audience = builder.Configuration.GetValue<string>("JwtSettings:Audience");
    var signingKey = builder.Configuration.GetValue<string>("JwtSettings:SigningKey");
    if (string.IsNullOrEmpty(signingKey))
    {
        throw new InvalidOperationException("JWT 簽名密鑰未配置、請在配置文件中設置 'JwtSettings:SigningKey");
    }
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<PasswordPolicy>(builder.Configuration.GetSection("PasswordPolicy"));
builder.Services.Configure<QueryRangeSettings>(builder.Configuration.GetSection("QueryRangeSettings"));
builder.Services.Configure<LockoutSettings>(builder.Configuration.GetSection("LockoutSettings"));
builder.Services.Configure<CertificateSettings>(builder.Configuration.GetSection("CertificateSettings"));
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<FileListChangeNotifier>();
builder.Services.AddSingleton<DigitalSignatureHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
