using ProgramGuard.Models;

namespace ProgramGuard.Interface.Service
{
    public interface IUserService
    {
        Task<bool> IsUserEnabledAsync(string account);
        Task<bool> IsUserLockedAsync(string account);
        Task<bool> IsUserDeletedAsync(string account);
        Task<(bool IsValid, string? ErrorMessage)> ValidateUserStatusAsync(string account);
        Task<bool> IsPasswordChangeRequiredAsync(string account);
    }
}
