namespace ProgramGuard.Dtos.User
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PrivilegeRuleName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}
