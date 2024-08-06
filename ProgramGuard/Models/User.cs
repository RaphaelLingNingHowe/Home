using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "超過可輸入上限(50)")]
        public string Account { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "超過可輸入上限(50)")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(128, ErrorMessage = "超過可輸入上限(128)")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號")]
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
