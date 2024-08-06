using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class OperateLog
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ACTION Action { get; set; }

        [StringLength(125, ErrorMessage = "超過可輸入上限(255)")]
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
}
