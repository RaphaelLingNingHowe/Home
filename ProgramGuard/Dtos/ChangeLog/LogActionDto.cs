using ProgramGuard.Enums;

namespace ProgramGuard.Dtos.ChangeLog
{
    public class LogActionDto
    {
        public ACTION Action { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
