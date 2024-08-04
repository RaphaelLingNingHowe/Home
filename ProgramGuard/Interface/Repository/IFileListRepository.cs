using ProgramGuard.Models;

namespace ProgramGuard.Interface.Repository
{
    public interface IFileListRepository : IRepository<FileList>
    {
        Task<IEnumerable<FileList>> GetAllFileListsAsync();
    }
}
