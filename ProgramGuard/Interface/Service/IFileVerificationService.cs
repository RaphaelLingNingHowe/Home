namespace ProgramGuard.Interface.Service
{
    public interface IFileVerificationService
    {
        string ComputeSha512(string filePath);
        bool VerifyDigitalSignature(string filePath);
        Task<bool> IsSha512SameAsync(string filePath);
    }
}
