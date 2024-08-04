using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Dtos.ChangeLog
{
    public class GetChangeLogDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public CHANGE_TYPE ChangeType { get; set; }
        public string? ChangeDetail { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool DigitalSIgnature { get; set; }
    }
}
