using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.User
{
    public class UpdateUserNameDto
    {
        [Required(ErrorMessage = "請輸入帳號名稱")]
        [MaxLength(16, ErrorMessage = "帳號名稱長度不能超過 16 個字元")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "名稱只能包含字母和數字")]
        public string Name { get; set; } = string.Empty;
    }
}
