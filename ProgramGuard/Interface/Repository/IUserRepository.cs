using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Interface.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByAccountAsync(string account);
        Task<User?> GetUserAsync(ClaimsPrincipal principal);
        string? GetUserAccount(ClaimsPrincipal principal);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }

}
