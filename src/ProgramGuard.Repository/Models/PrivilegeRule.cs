using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Repository.Models
{
    public class PrivilegeRule
    {
        public int Id { get; set; }

        [MaxLength(16)]
        public string Name { get; set; } = string.Empty;
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
