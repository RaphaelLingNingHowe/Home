using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ProgramGuardContext context) : base(context)
        {
        }

        public async Task<User?> GetByAccountAsync(string account)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Account == account);
        }
        public async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            var account = GetUserAccount(principal);
            return account != null ? await GetByAccountAsync(account) : null;
        }

        public string? GetUserAccount(ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.PrivilegeRule)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

    }
}
