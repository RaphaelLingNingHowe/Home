using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Dtos.User;
using ProgramGuard.Helper;
using ProgramGuard.Models;
using System.Security.Principal;

namespace ProgramGuard.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ProgramGuardContext context, ILogger<BaseController> logger) : base(context, logger)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAlUsersAsync()
        {
            IEnumerable<GetUserDto> result = await _context.Users
                        .OrderBy(u => u.Id)
                        .Select(u => new GetUserDto
                        {
                            Id = u.Id,
                            Account = u.Account,
                            Name = u.Name,
                            PrivilegeRuleName = u.PrivilegeRule.Name,
                            IsEnabled = u.IsEnabled,
                            IsDeleted = u.IsDeleted,
                            IsLocked = u.IsLocked,
                            LockoutEnd = u.LockoutEnd
                        })
                        .ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            if (await _context.Users.AnyAsync(u => u.Account == createUserDto.Account))
            {
                return Forbidden($"已有此帳號-[{createUserDto.Account}]");
            }
            User user = new()
            {
                Account = createUserDto.Account,
                Name = createUserDto.Account,
                Password = PasswordHelper.HashPassword(createUserDto.Password),
                PrivilegeRuleId = 1
            };
            await _context.AddAsync(user);
            return Created(user.Id);
        }

        [HttpPatch("{account}/privilege")]
        public async Task<IActionResult> UpdateUserPrivilegeRuleAsync(string account, [FromBody] int privilegeRuleId)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            else if (await _context.PrivilegeRules.FindAsync(privilegeRuleId) is not PrivilegeRule privilegeRule)
            {
                return NotFound($"找不到此權限-[{privilegeRuleId}]");
            }
            else if (user.PrivilegeRuleId == privilegeRuleId)
            {
                return Forbidden($"帳號已有此權限-[{privilegeRule.Name}]");
            }
            user.PrivilegeRuleId = privilegeRuleId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{account}/enable")]
        public async Task<IActionResult> EnableUserAsync(string account)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            user.IsEnabled = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{account}/disable")]
        public async Task<IActionResult> DisableUserAsync(string account)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            user.IsEnabled = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{account}/unlock")]
        public async Task<IActionResult> UnlockUserAsync(string account)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            user.IsLocked = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{account}/reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(string account, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            user.Password = PasswordHelper.HashPassword(resetPasswordDto.Password);
            user.LastPasswordChanged = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{account}")]
        public async Task<IActionResult> DeleteUserAsync(string account)
        {
            if (await _context.Users.SingleOrDefaultAsync(u => u.Account == account) is not User user)
            {
                return NotFound($"找不到此帳號-[{account}]");
            }
            else if (user.IsDeleted)
            {
                return Forbidden($"此帳號已刪除-[{user.Account}]");
            }
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
