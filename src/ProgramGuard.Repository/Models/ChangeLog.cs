using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Repository.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }

        [ForeignKey("FileList")]
        public int FileListId { get; set; }
        public FileList FileList { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool DigitalSignature { get; set; }

        [MaxLength(128)]
        public string? Sha512 { get; set; }
    }
}
