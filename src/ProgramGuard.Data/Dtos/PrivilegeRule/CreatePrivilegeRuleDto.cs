using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.PrivilegeRule
{
    public class CreatePrivilegeRuleDto
    {
        [Required(ErrorMessage = "請輸入權限規則名稱")]
        [MaxLength(16, ErrorMessage = "規則名稱長度不能超過 16 個字元")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "名稱只能包含字母和數字")]
        public string Name { get; set; } = string.Empty;

        public VISIBLE_PRIVILEGE Visible { get; set; }

        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
