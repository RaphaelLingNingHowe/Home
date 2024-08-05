using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }

        [ForeignKey("FileList")]
        public int FileListId { get; set; }
        public FileList FileList { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public CHANGE_TYPE ChangeType { get; set; }
        public string? ChangeDetail { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool DigitalSignature { get; set; }
        public string? Sha512 { get; set; }
    }
}
