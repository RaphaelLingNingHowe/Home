using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "請輸入帳號")]
        [MaxLength(16, ErrorMessage = "帳號長度不能超過 16 個字元")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [MaxLength(128, ErrorMessage = "密碼長度不能超過 128 個字元")]
        public string Password { get; set; } = string.Empty;
    }
}
