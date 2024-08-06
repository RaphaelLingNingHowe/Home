using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(128, ErrorMessage = "超過可輸入上限(128)")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(128, ErrorMessage = "超過可輸入上限(128)")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於(8)")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入確認密碼")]
        [StringLength(128, ErrorMessage = "超過可輸入上限(128)")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
