using ProgramGuard.Enums;

namespace ProgramGuard.Data.Dtos.PrivilegeRule
{
    public class UpdatePrivilegeRuleDto
    {
        public VISIBLE_PRIVILEGE Visible { get; set; }

        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
