using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProgramGuard.Config;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Interface.Service;
using ProgramGuard.Models;
using ProgramGuard.Settings;
using System.Security.Principal;

namespace ProgramGuard.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;
        public UserService(IUserRepository userRepository, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }
        public async Task<bool> IsUserEnabledAsync(string account)
        {
            var user = await _userRepository.GetByAccountAsync(account);
            return user != null && user.IsEnabled;
        }
        public async Task<bool> IsUserLockedAsync(string account)
        {
            var user = await _userRepository.GetByAccountAsync(account);
            return user != null && user.IsLocked;
        }

        public async Task<bool> IsUserDeletedAsync(string account)
        {
            var user = await _userRepository.GetByAccountAsync(account);
            return user != null && user.IsDeleted;
        }
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateUserStatusAsync(string account)
        {
            if (await IsUserDeletedAsync(account))
                return (false, "User account has been deleted.");
            if (await IsUserLockedAsync(account))
                return (false, "User account is locked.");
            if (!await IsUserEnabledAsync(account))
                return (false, "User account is disabled.");

            return (true, null);
        }
        public async Task<bool> IsPasswordChangeRequiredAsync(string account)
        {
            var user = await _userRepository.GetByAccountAsync(account);
            if (user == null)
                return false;

            int maxPasswordAgeDays = _appSettings.PasswordPolicy.PasswordChangeDays;
            return (DateTime.Now - user.LastPasswordChangedDate).TotalDays > maxPasswordAgeDays;
        }
    }
}
