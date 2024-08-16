namespace ProgramGuard.Data.Config
{
    public class LockoutSettings
    {
        public int LockoutInMinutes { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
