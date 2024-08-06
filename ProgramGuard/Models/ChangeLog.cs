using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;
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

        [StringLength(125, ErrorMessage = "超過可輸入上限(255)")]
        public string? ChangeDetail { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool DigitalSignature { get; set; }

        [StringLength(128, ErrorMessage = "超過可輸入上限(128)")]
        public string? Sha512 { get; set; }
    }
}
