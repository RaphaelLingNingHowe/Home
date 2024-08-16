using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.User
{
    public class ResetPasswordDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入新密碼")]
        [MaxLength(128, ErrorMessage = "新密碼長度不能超過 128 個字元")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於 8 個字元")]
        public string Password { get; set; } = string.Empty;
    }
}
