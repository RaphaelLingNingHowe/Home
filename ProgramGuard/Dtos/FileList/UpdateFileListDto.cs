using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileList
{
    public class UpdateFileListDto
    {
        [Required]
        public string Path { get; set; } = string.Empty;
    }
}
