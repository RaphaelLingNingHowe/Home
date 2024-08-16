using ProgramGuard.Data.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Helper;

namespace ProgramGuard.Services
{
    public class Privilege
    {
        public static readonly PrivilegesDto Privileges;

        static Privilege()
        {
            Privileges = new PrivilegesDto
            {
                VisiblePrivileges = GetVisiblePrivileges(),
                OperatePrivileges = GetOperatePrivileges()
            };
        }

        private static IEnumerable<VisiblePrivilegeDto> GetVisiblePrivileges()
        {
            return Enum.GetValues(typeof(VISIBLE_PRIVILEGE))
                .Cast<VISIBLE_PRIVILEGE>()
                .Where(p => p != VISIBLE_PRIVILEGE.UNKNOWN)
                .Select(p => new VisiblePrivilegeDto
                {
                    Value = (uint)p,
                    Name = p.ToString(),
                    Description = EnumHelper.GetEnumDescription(p)
                })
                .ToList();
        }

        private static IEnumerable<OperatePrivilegeDto> GetOperatePrivileges()
        {
            return Enum.GetValues(typeof(OPERATE_PRIVILEGE))
                .Cast<OPERATE_PRIVILEGE>()
                .Where(p => p != OPERATE_PRIVILEGE.UNKNOWN)
                .Select(p => new OperatePrivilegeDto
                {
                    Value = (uint)p,
                    Name = p.ToString(),
                    Description = EnumHelper.GetEnumDescription(p)
                })
                .ToList();
        }
    }
}
