using ProgramGuard.Config;
using System.Security.Cryptography.X509Certificates;

namespace ProgramGuard.Helper
{
    public class DigitalSignatureHelper
    {
        private readonly CertificateSettings _certificateSettings;
        private readonly ILogger<DigitalSignatureHelper> _logger;

        public DigitalSignatureHelper(CertificateSettings certificateSettings, ILogger<DigitalSignatureHelper> logger)
        {
            _certificateSettings = certificateSettings;
            _logger = logger;
        }

        public bool VerifyDigitalSignature(string filePath)
        {
            try
            {
                using var referenceCertificate = new X509Certificate2(_certificateSettings.CertificatePath);
                using var currentCertificate = new X509Certificate2(filePath);
                return referenceCertificate.Thumbprint == currentCertificate.Thumbprint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying digital signature");
                return false;
            }
        }
    }
}
