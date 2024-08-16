using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.User
{
    public class CreateUserDto
    {
        [Key]
        [Required(ErrorMessage = "請輸入帳號")]
        [MaxLength(16, ErrorMessage = "帳號長度不能超過 16 個字元")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "帳號只能包含字母和數字")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入帳號名稱")]
        [MaxLength(16, ErrorMessage = "帳號名稱長度不能超過 16 個字元")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "名稱只能包含字母和數字")]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入密碼")]
        [MaxLength(128, ErrorMessage = "密碼長度不能超過 128 個字元")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於 8 個字元")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int PrivilegeRuleId { get; set; }
    }
}
