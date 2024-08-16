namespace ProgramGuard.Data.Dtos.PrivilegeRule
{
    public class PrivilegesDto
    {
        public IEnumerable<VisiblePrivilegeDto>? VisiblePrivileges { get; set; }

        public IEnumerable<OperatePrivilegeDto>? OperatePrivileges { get; set; }
    }

    public class VisiblePrivilegeDto
    {
        public uint Value { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class OperatePrivilegeDto
    {
        public uint Value { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
