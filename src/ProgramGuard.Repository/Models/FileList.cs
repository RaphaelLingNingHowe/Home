using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Repository.Models
{
    public class FileList
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Path { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
