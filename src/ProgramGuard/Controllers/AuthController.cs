using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Data.Config;
using ProgramGuard.Data.Dtos.Auth;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;

namespace ProgramGuard.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly LockoutSettings _lockoutSettings;
        private readonly PasswordPolicy _passwordPolicy;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<LockoutSettings> lockoutSettings, IOptions<PasswordPolicy> options, JwtHelper jwtHelper) : base(context, logger)
        {
            _lockoutSettings = lockoutSettings.Value;
            _passwordPolicy = options.Value;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == dto.Account) is not User user)
            {
                return Forbidden("帳號或密碼錯誤");
            }
            else if (!user.IsEnabled)
            {
                string message = "帳號已停用、請洽管理員";
                await LogActionAsync(user, ACTION.LOGIN, message);
                return Forbidden(message);
            }
            else if (user.IsDeleted)
            {
                string message = "此帳號已刪除";
                await LogActionAsync(user, ACTION.LOGIN, message);
                return Forbidden(message);
            }
            else if (user.IsLocked && user.LockoutEnd > DateTime.Now)
            {
                string message = $"此帳號已鎖定，解鎖時間-[{user.LockoutEnd}]";
                await LogActionAsync(user, ACTION.LOGIN, message);
                return Forbidden(message);
            }
            else
            {
                if (user.IsLocked)
                {
                    user.IsLocked = false;
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    await _context.SaveChangesAsync();
                }

                if (!PasswordHelper.VerifyPassword(dto.Password, user.Password))
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= _lockoutSettings.AccessFailedCount)
                    {
                        user.IsLocked = true;
                        user.LockoutEnd = DateTime.Now.AddMinutes(_lockoutSettings.LockoutInMinutes);
                        await _context.SaveChangesAsync();
                        string message = $"帳號因多次登入失敗而被鎖定，請在{user.LockoutEnd}後再試";
                        await LogActionAsync(user, ACTION.LOGIN, message);
                        return Forbidden(message);
                    }
                    await _context.SaveChangesAsync();
                    return Forbidden("帳號或密碼錯誤");
                }
                else
                {
                    user.AccessFailedCount = 0;
                    if ((DateTime.Now - user.LastPasswordChanged).TotalDays > _passwordPolicy.PasswordChangeDays)
                    {
                        string message = $"超過{_passwordPolicy.PasswordChangeDays}天未更換密碼，需要更換密碼";
                        LoginResponseDto result = new()
                        {
                            Token = _jwtHelper.GenerateChangePasswordOnlyToken(user),
                            Message = message
                        };
                        await LogActionAsync(user, ACTION.LOGIN, message);
                        return Ok(result);
                    }
                    else
                    {
                        await _context.Entry(user).Reference(u => u.PrivilegeRule).LoadAsync();
                        LoginResponseDto result = new()
                        {
                            Token = _jwtHelper.GenerateToken(user)
                        };
                        await LogActionAsync(user, ACTION.LOGIN);
                        return Ok(result);
                    }
                }
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await LogActionAsync(ACTION.LOGOUT);
            return NoContent();
        }
    }
}
