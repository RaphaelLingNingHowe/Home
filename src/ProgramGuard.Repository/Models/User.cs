using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Repository.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(16)]
        public string Account { get; set; } = string.Empty;

        [MaxLength(16)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;

        [ForeignKey("PrivilegeRule")]
        public int PrivilegeRuleId { get; set; }
        public PrivilegeRule PrivilegeRule { get; set; } = null!;
        public DateTime LastPasswordChanged { get; set; } = DateTime.Now;
        public bool IsEnabled { get; set; } = true;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public byte AccessFailedCount { get; set; }

    }
}
