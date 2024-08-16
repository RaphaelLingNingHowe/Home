using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.User
{
    public class ChangePasswordDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入密碼")]
        [MaxLength(128, ErrorMessage = "密碼長度不能超過 128 個字元")]
        public string CurrentPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入新密碼")]
        [MaxLength(128, ErrorMessage = "新密碼長度不能超過 128 個字元")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於 8 個字元")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入確認密碼")]
        [MaxLength(128, ErrorMessage = "確認密碼長度不能超過 128 個字元")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
