using Microsoft.AspNetCore.Identity;

namespace ProgramGuard.Config
{
    public class AppSettings
    {
        public PasswordPolicy PasswordPolicy { get; set; }
        public TimeRangeSettings TimeRangeSettings { get; set; }
        public WorkerSettings WorkerSettings { get; set; }
        public LockoutSettings LockoutSettings { get; set; }
        public CertificateSettings CertificateSettings { get; set; }
    }
}
