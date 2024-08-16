﻿namespace ProgramGuard.Data.Dtos.ChangeLog
{
    public class GetChangeLogDto
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string UserName { get; set; } = string.Empty;

        public bool IsConfirmed { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public bool DigitalSignature { get; set; }
    }
}