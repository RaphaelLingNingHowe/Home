using Microsoft.Extensions.Options;
using ProgramGuard.Config;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Interface.Service;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ProgramGuard.Services
{
    public class FileVerificationService : IFileVerificationService
    {
        private readonly CertificateSettings _certificateSettings;
        private readonly IChangeLogRepository _changeLogRepository;
        public FileVerificationService(IOptions<CertificateSettings> certificateSettings, IChangeLogRepository changeLogRepository)
        {
            _certificateSettings = certificateSettings.Value;
            _changeLogRepository = changeLogRepository;
        }
        public string ComputeSha512(string filePath)
        {
            using var sha512 = SHA512.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = sha512.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public async Task<bool> IsSha512SameAsync(string filePath)
        {
            var oldSha512 = await _changeLogRepository.GetLatestSha512Async(filePath);
            var newSha512 = ComputeSha512(filePath);

            return newSha512 == oldSha512;
        }

        public bool VerifyDigitalSignature(string filePath)
        {
            using var referenceCertificate = new X509Certificate2(_certificateSettings.CertificatePath);
            using var currentCertificate = new X509Certificate2(filePath);
            return referenceCertificate.Thumbprint == currentCertificate.Thumbprint;
        }
    }
}
