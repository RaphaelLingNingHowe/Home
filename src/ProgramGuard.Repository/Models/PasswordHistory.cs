using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Repository.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
