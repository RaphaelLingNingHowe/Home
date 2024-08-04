using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class FileListRepository : Repository<FileList>, IFileListRepository
    {
        public FileListRepository(ProgramGuardContext context) : base(context)
        {
        }
        public async Task<IEnumerable<FileList>> GetAllFileListsAsync()
        {
            return await _context.FileLists
                .Where(f => !f.IsDeleted)
                .ToListAsync();
        }
    }
}
