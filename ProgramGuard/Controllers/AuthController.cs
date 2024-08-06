using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Auth;
using ProgramGuard.Dtos.ChangeLog;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Models;
using System.Security.Principal;

namespace ProgramGuard.Controllers
{
    public class AuthController : BaseController
    {
        private readonly LockoutSettings _lockoutSettings;
        private readonly PasswordPolicy _passwordPolicy;
        private readonly JwtHelper _jwtHelper;
        public AuthController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<LockoutSettings> lockoutSettings, IOptions<PasswordPolicy> passwordPolicy, JwtHelper jwtHelper) : base(context, logger)
        {
            _lockoutSettings = lockoutSettings.Value;
            _passwordPolicy = passwordPolicy.Value;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == loginDto.Account) is not User user)
            {
                return Forbidden("帳號或密碼錯誤");
            }
            else if (!user.IsEnabled)
            {
                return Forbidden($"此帳號已停用-[{user.Account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            else if (user.IsLocked && user.LockoutEnd > DateTime.Now)
            {
                return Forbidden($"此帳號已鎖定，解鎖時間-[{user.LockoutEnd}]");
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
                if (!PasswordHelper.VerifyPassword(loginDto.Password, user.Password))
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= _lockoutSettings.LockoutCount)
                    {
                        user.IsLocked = true;
                        user.LockoutEnd = DateTime.Now.AddMinutes(_lockoutSettings.LockoutInMinutes);
                        await _context.SaveChangesAsync();
                        return Forbidden($"帳號因多次登入失敗而被鎖定，請在{user.LockoutEnd}後再試");
                    }
                    await _context.SaveChangesAsync();
                    return Forbidden("帳號或密碼錯誤");
                }
                else
                {
                    if ((DateTime.Now - user.LastPasswordChanged).TotalDays > _passwordPolicy.PasswordChangeDays)
                    {
                        LoginResponseDto result = new()
                        {
                            RequirePasswordChange = true,
                            Token = _jwtHelper.GenerateChangePasswordOnlyToken(user),
                            Message = $"超過{_passwordPolicy.PasswordChangeDays}天未更換密碼，需要更換密碼"
                        };
                        return Ok(result);
                    }
                    else
                    {
                        LoginResponseDto result = new()
                        {
                            Token = _jwtHelper.GenerateToken(user),
                            Message = $"登入成功"
                        };
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

        [Authorize]
        [HttpPost("{account}/change-password")]
        public async Task<IActionResult> ChangePasswordAsync(string account, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (!user.IsEnabled)
            {
                return Forbidden($"此帳號已停用-[{user.Account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            else if (user.IsLocked && user.LockoutEnd > DateTime.Now)
            {
                return Forbidden($"此帳號已鎖定，解鎖時間-[{user.LockoutEnd}]");
            }
            if (!PasswordHelper.VerifyPassword(changePasswordDto.CurrentPassword, user.Password))
            {
                return Forbidden("帳號或密碼錯誤");
            }
            user.Password = PasswordHelper.HashPassword(changePasswordDto.NewPassword);
            user.LastPasswordChanged = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("密码更改成功");
        }
    }
}
