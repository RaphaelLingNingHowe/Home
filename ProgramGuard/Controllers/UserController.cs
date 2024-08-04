using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Interface.Service;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    public class UserController : BaseController
    {
        private readonly IPasswordHasherService _passwordHasherService;
        public UserController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository, IPasswordHasherService passwordHasherService) : base(context, logger, userRepository, operateLogRepository)
        {
            _passwordHasherService = passwordHasherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(u => new GetUserDto
            {
                Id = u.Id,
                Account = u.Account,
                Name = u.Name,
                PrivilegeRuleName = u.PrivilegeRule.Name,
                IsEnabled = u.IsEnabled,
                IsDeleted = u.IsDeleted,
                IsLocked = u.IsLocked,
                LockoutEnd = u.LockoutEnd
            });
            return Ok(userDtos);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            User user = new()
            {
                Account = createUserDto.Account,
                Name = createUserDto.Account,
                Password = createUserDto.Password,
                PrivilegeRuleId = 1
            };
            await _userRepository.AddAsync(user);
            return Created();
        }

        [HttpPatch("{id}/privilege")]
        public async Task<ActionResult> UpdateUserPrivilegeRuleAsync(int id, [FromBody] int privilegeRuleId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.PrivilegeRuleId = privilegeRuleId;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPatch("{id}/enable")]
        public async Task<ActionResult> EnableUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsEnabled = true;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPatch("{id}/disable")]
        public async Task<ActionResult> DisableUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsEnabled = false;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPatch("{id}/unlock")]
        public async Task<ActionResult> UnlockUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsLocked = false;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPatch("{id}/reset-password")]
        public async Task<ActionResult> ResetPasswordAsync(int id, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Password = _passwordHasherService.HashPassword(resetPasswordDto.Password);
            user.LastPasswordChangedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
