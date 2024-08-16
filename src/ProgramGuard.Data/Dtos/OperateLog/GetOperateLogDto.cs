namespace ProgramGuard.Data.Dtos.OperateLog
{
    public class GetOperateLogDto
    {
        public int Id { get; set; }

        public string Account { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
