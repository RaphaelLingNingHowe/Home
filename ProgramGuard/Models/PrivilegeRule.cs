using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Models
{
    public class PrivilegeRule
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "超過可輸入上限(50)")]
        public string Name { get; set; } = string.Empty;
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
