using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Enums;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Interface.Service;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    public class FileListController : BaseController
    {
        private readonly IFileListRepository _fileListRepository;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IFileVerificationService _fileVerificationService;
        public FileListController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository, IFileListRepository fileListRepository, IChangeLogRepository changeLogRepository, IFileVerificationService fileVerificationService) : base(context, logger, userRepository, operateLogRepository)
        {
            _fileListRepository = fileListRepository;
            _changeLogRepository = changeLogRepository;
            _fileVerificationService = fileVerificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetFileListDto>>> GetAllFileListsAsync()
        {
            var fileLists = await _fileListRepository.GetAllFileListsAsync();
            var fileListDtos = fileLists.Select(f => new GetFileListDto
            {
                Id = f.Id,
                Name = f.Name,
                Path = f.Path,
            });
            return Ok(fileListDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CreateFileListDto>> CreateFileList([FromBody] CreateFileListDto createFileListDto)
        {
            FileList fileList = new()
            {
                Name = Path.GetFileName(createFileListDto.Path),
                Path = createFileListDto.Path
            };
            await _fileListRepository.AddAsync(fileList);
            ChangeLog changeLog = new()
            {
                FileListId = fileList.Id,
                ChangeType = CHANGE_TYPE.CREATED,
                DigitalSIgnature = _fileVerificationService.VerifyDigitalSignature(createFileListDto.Path),
                Sha512 = _fileVerificationService.ComputeSha512(createFileListDto.Path)
            };
            await _changeLogRepository.AddAsync(changeLog);
            return Created();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateFileListAsync(int id, [FromBody] UpdateFileListDto updateFileListDto)
        {
            var fileList = await _fileListRepository.GetByIdAsync(id);
            if (fileList == null)
            {
                return NotFound();
            }
            fileList.Name = Path.GetFileName(updateFileListDto.Path);
            fileList.Path = updateFileListDto.Path;
            await _fileListRepository.UpdateAsync(fileList);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileList(int id)
        {
            var fileList = await _fileListRepository.GetByIdAsync(id);
            if (fileList == null)
            {
                return NotFound();
            }
            fileList.IsDeleted = true;
            await _fileListRepository.UpdateAsync(fileList);
            return NoContent();
        }
    }
}
