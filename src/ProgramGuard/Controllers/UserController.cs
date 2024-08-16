using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProgramGuard.Base;
using ProgramGuard.Data.Config;
using ProgramGuard.Data.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;

namespace ProgramGuard.Controllers
{
    public class UserController : BaseController
    {
        private readonly PasswordPolicy _passwordPolicy;
        public UserController(ProgramGuardContext context, ILogger<BaseController> logger, IOptions<PasswordPolicy> options) : base(context, logger)
        {
            _passwordPolicy = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlUsersAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_USER))
            {
                return Forbidden("權限不足，無法查看");
            }

            IEnumerable<GetUserDto> result = await _context.Users
                        .Where(u => !u.IsDeleted)
                        .OrderBy(u => u.Id)
                        .Select(u => new GetUserDto
                        {
                            Id = u.Id,
                            Account = u.Account,
                            Name = u.Name,
                            PrivilegeRule = u.PrivilegeRuleId,
                            IsEnabled = u.IsEnabled,
                            IsLocked = u.IsLocked,
                            LockoutEnd = u.LockoutEnd
                        })
                        .ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.ADD_ACCOUNT))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.AnyAsync(u => u.Account == dto.Account))
            {
                return Forbidden($"已有此帳號-[{dto.Account}]");
            }

            User user = new()
            {
                Account = dto.Account,
                Name = dto.Account,
                Password = PasswordHelper.HashPassword(dto.Password),
                PrivilegeRuleId = dto.PrivilegeRuleId
            };

            await _context.AddAsync(user);
            await LogActionAsync(ACTION.CREATE_ACCOUNT, $"帳號-[{dto.Account}]");

            return Created(user.Id);
        }

        [HttpPost("{account}/change-password")]
        public async Task<IActionResult> ChangePasswordAsync(string account, [FromBody] ChangePasswordDto dto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }

            // TODO 確認能否這樣比較, 這樣比較應該是不準確的, 因為這樣應該是比較到兩個不同記憶體位置的物件
            if (await GetUserAsync(User) != user)
            {
                string message = $"不能更換此帳號-[{account}]的密碼";
                await LogActionAsync(ACTION.CHANGE_PASSWORD, message);

                return Forbidden(message);
            }

            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.Password))
            {
                string message = "帳號或密碼錯誤";
                await LogActionAsync(ACTION.CHANGE_PASSWORD, message);

                return Forbidden(message);
            }
            else
            {
                if (await _context.PasswordHistories.SingleOrDefaultAsync(p => p.User == user) is not PasswordHistory passwordHistory)
                {
                    _logger.LogError("未找到此帳號-[{account}]的密碼記錄", account);
                    return ServerError("密碼更換異常");
                }
                else
                {
                    var newPasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
                    var passwordHistoryCount = _passwordPolicy.PasswordHistoryCount;
                    var isPasswordInHistory = await _context.PasswordHistories
                        .Where(ph => ph.UserId == user.Id)
                        .OrderByDescending(ph => ph.Timestamp)
                        .Take(passwordHistoryCount)
                        .AnyAsync(ph => ph.Password == newPasswordHash);

                    if (isPasswordInHistory)
                    {
                        return Forbidden($"新密碼不能與最近{passwordHistoryCount}次使用的密碼相同");
                    }

                    user.Password = PasswordHelper.HashPassword(dto.NewPassword);
                    user.LastPasswordChanged = DateTime.Now;
                    passwordHistory.UserId = user.Id;
                    passwordHistory.Password = newPasswordHash;

                    await _context.SaveChangesAsync();
                    await LogActionAsync(ACTION.CHANGE_PASSWORD);

                    return NoContent();
                }
            }
        }

        [HttpPost("{account}/reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(string account, [FromBody] ResetPasswordDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.RESET_PASSWORD))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.RESET_PASSWORD, message);
                return Forbidden(message);
            }

            user.Password = PasswordHelper.HashPassword(dto.Password);
            user.LastPasswordChanged = DateTime.Now;

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.RESET_PASSWORD, $"帳號-[{account}]");

            return NoContent();
        }

        [HttpPatch("{account}/name")]
        public async Task<IActionResult> UpdateUserNameAsync(string account, [FromBody] UpdateUserNameDto dto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            else if (user.Name == dto.Name)
            {
                return NoContent();
            }

            if (await _context.Users.AnyAsync(u => u.Name == dto.Name && u.Account != account))
            {
                return Forbidden("此名稱已被使用");
            }

            user.Name = dto.Name;
            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.MODIFY_DISPLAY_NAME, $"[{user.Name}]變更為[{dto.Name}]");

            return NoContent();
        }

        [HttpPatch("{account}/privilege")]
        public async Task<IActionResult> UpdateUserPrivilegeRuleAsync(string account, [FromBody] UpdateUserPrivilegeDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.MODIFY_PRIVILEGE, message);
                return Forbidden(message);
            }
            else if (await _context.PrivilegeRules.FindAsync(dto.PrivilegeRuleId) is not PrivilegeRule privilegeRule)
            {
                return NotFound($"找不到此權限-[{dto.PrivilegeRuleId}]");
            }
            else if (user.PrivilegeRuleId == dto.PrivilegeRuleId)
            {
                return Forbidden($"帳號已有此權限-[{privilegeRule.Name}]");
            }

            user.PrivilegeRuleId = dto.PrivilegeRuleId;
            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.MODIFY_PRIVILEGE, $"帳號-[{account}]");

            return NoContent();
        }

        [HttpPatch("{account}/enable")]
        public async Task<IActionResult> EnableUserAsync(string account)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.ENABLE_ACCOUNT))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.ENABLE_ACCOUNT, message);
                return Forbidden(message);
            }

            user.IsEnabled = true;

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.ENABLE_ACCOUNT, $"帳號-[{account}]");

            return NoContent();
        }

        [HttpPatch("{account}/disable")]
        public async Task<IActionResult> DisableUserAsync(string account)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.DISABLE_ACCOUNT))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.DISABLE_ACCOUNT, message);
                return Forbidden(message);
            }

            user.IsEnabled = false;

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.DISABLE_ACCOUNT, $"帳號-[{account}]");
            return NoContent();
        }

        [HttpPatch("{account}/unlock")]
        public async Task<IActionResult> UnlockUserAsync(string account)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.UNLOCK_ACCOUNT))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.UNLOCK_ACCOUNT, message);
                return Forbidden(message);
            }

            user.IsLocked = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.UNLOCK_ACCOUNT, $"帳號-[{account}]");

            return NoContent();
        }

        [HttpDelete("{account}")]
        public async Task<IActionResult> DeleteUserAsync(string account)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.DELETE_ACCOUNT))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                string message = $"此帳號已刪除-[{user.Account}]";
                await LogActionAsync(ACTION.DELETE_ACCOUNT, message);
                return Forbidden(message);
            }

            user.IsDeleted = true;

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.DELETE_ACCOUNT, $"帳號-[{account}]");
            return NoContent();
        }
    }
}
