using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [ForeignKey("PrivilegeRule")]
        public int PrivilegeRuleId { get; set; }
        public PrivilegeRule PrivilegeRule { get; set; } = null!;
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.Now;
        public bool IsEnabled { get; set; } = true;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockoutEnd { get; set; }
        public byte AccessFailedCount { get; set; }

    }
}
