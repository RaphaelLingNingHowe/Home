using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Repository.Models
{
    public class OperateLog
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ACTION Action { get; set; }

        [MaxLength(525)]
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
}
