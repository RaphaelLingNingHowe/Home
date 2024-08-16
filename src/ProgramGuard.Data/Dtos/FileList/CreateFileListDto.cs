using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Data.Dtos.FileList
{
    public class CreateFileListDto
    {
        [Required(ErrorMessage = "請輸入檔案路徑")]
        [MaxLength(512, ErrorMessage = "檔案路徑長度不能超過 256 個字元")]
        [RegularExpression(@"^(?:[a-zA-Z]:\\|\\/)?(?:[\w\-. @()\u4E00-\u9FFF]+[\\/])*[\w\-. @()\u4E00-\u9FFF]*$", ErrorMessage = "請輸入有效的檔案路徑")]
        public string Path { get; set; } = string.Empty;
    }
}
