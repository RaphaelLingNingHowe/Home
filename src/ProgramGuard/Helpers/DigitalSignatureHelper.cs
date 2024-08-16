using Microsoft.Extensions.Options;
using ProgramGuard.Data.Config;
using System.Security.Cryptography.X509Certificates;

namespace ProgramGuard.Helper
{
    public class DigitalSignatureHelper
    {
        private readonly CertificateSettings _options;
        private readonly ILogger<DigitalSignatureHelper> _logger;

        public DigitalSignatureHelper(IOptions<CertificateSettings> options, ILogger<DigitalSignatureHelper> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public bool VerifyDigitalSignature(string filePath)
        {
            try
            {
                // TODO 憑證驗證的方式需要再討論, 如果要偵測其他廠商的軟體那憑證如何處理, 以及憑證到期後, 更換新憑證 Thumbprint 會不同
                using var referenceCertificate = new X509Certificate2(_options.CertificatePath);
                using var currentCertificate = new X509Certificate2(filePath);
                return referenceCertificate.Thumbprint == currentCertificate.Thumbprint;
            }
            catch
            {
                return false;
            }
        }
    }
}
