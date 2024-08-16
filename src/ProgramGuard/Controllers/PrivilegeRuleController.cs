using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;

namespace ProgramGuard.Controllers
{
    public class PrivilegeRuleController : BaseController
    {
        public PrivilegeRuleController(ProgramGuardContext context, ILogger<BaseController> logger) : base(context, logger)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrivilegeRulesAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_PRIVILEGE_RULE))
            {
                return Forbidden("權限不足，無法查看");
            }

            IEnumerable<GetPrivilegeRuleDto> result = await _context.PrivilegeRules
                        .OrderBy(p => p.Id)
                        .Select(p => new GetPrivilegeRuleDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Visible = p.Visible,
                            Operate = p.Operate
                        })
                        .ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivilegeRuleAsync(CreatePrivilegeRuleDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.PrivilegeRules.AnyAsync(o => o.Name == dto.Name))
            {
                return NotFound($"名稱已被使用-[{dto.Name}]");
            }

            if (await _context.PrivilegeRules.SingleOrDefaultAsync(o => o.Visible == dto.Visible && o.Operate == dto.Operate) is PrivilegeRule duplicate)
            {
                return Forbidden($"已具有相同規則-[{duplicate.Name}]");
            }

            PrivilegeRule privilegeRule = new()
            {
                Name = dto.Name,
                Visible = dto.Visible,
                Operate = dto.Operate
            };

            await _context.AddAsync(privilegeRule);
            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.ADD_PRIVILEGE_RULE, $"權限名稱-[{privilegeRule.Name}]");

            return Created(privilegeRule.Id);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePrivilegeRuleAsync(int id, [FromBody] UpdatePrivilegeRuleDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.PrivilegeRules.FindAsync(id) is not PrivilegeRule privilegeRule)
            {
                return NotFound($"找不到此權限規則-[{id}]");
            }
            else if (privilegeRule.Visible == dto.Visible && privilegeRule.Operate == dto.Operate)
            {
                return Forbidden($"已有此權限規則-[{privilegeRule.Name}]");
            }

            privilegeRule.Visible = dto.Visible;
            privilegeRule.Operate = dto.Operate;

            await LogActionAsync(ACTION.MODIFY_PRIVILEGE_RULE, $"權限名稱-[{privilegeRule.Name}]");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrivilegeRuleAsync(int id)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.PrivilegeRules.FindAsync(id) is not PrivilegeRule privilegeRule)
            {
                return NotFound($"找不到此權限規則-[{id}]");
            }
            else if (await _context.Users.AnyAsync(u => u.PrivilegeRuleId == privilegeRule.Id))
            {
                return Forbidden($"此規則已被帳號引用、無法刪除");
            }

            _context.Remove(privilegeRule);

            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.DELETE_PRIVILEGE_RULE, $"權限名稱-[{privilegeRule.Name}]");

            return NoContent();
        }
    }
}
