using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Models;
using System.Security.Principal;

namespace ProgramGuard.Controllers
{
    public class FileListController : BaseController
    {
        private readonly DigitalSignatureHelper _digitalSignatureHelper;
        public FileListController(ProgramGuardContext context, ILogger<BaseController> logger, DigitalSignatureHelper digitalSignatureHelper) : base(context, logger)
        {
            _digitalSignatureHelper = digitalSignatureHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFileListsAsync()
        {
            IEnumerable<GetFileListDto> result = await _context.FileLists
                        .Where(f => !f.IsDeleted)
                        .OrderBy(f => f.Id)
                        .Select(f => new GetFileListDto
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Path = f.Path
                        })
                        .ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFileListAsync(CreateFileListDto createFileListDto)
        {
            if (await _context.FileLists.AnyAsync(f => f.Path == createFileListDto.Path))
            {
                return Forbidden($"已有此檔案路徑-[{createFileListDto.Path}]");
            }
            FileList fileList = new()
            {
                Name = Path.GetFileName(createFileListDto.Path),
                Path = createFileListDto.Path
            };
            await _context.AddAsync(fileList);
            await _context.SaveChangesAsync();
            ChangeLog changeLog = new()
            {
                FileListId = fileList.Id,
                ChangeType = CHANGE_TYPE.CREATED,
                DigitalSignature = _digitalSignatureHelper.VerifyDigitalSignature(createFileListDto.Path),
                Sha512 = FileHashHelper.ComputeSHA512Hash(createFileListDto.Path)
            };
            await _context.AddAsync(changeLog);
            await _context.SaveChangesAsync();
            return Created(fileList.Id);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateFileListAsync(int id, [FromBody] UpdateFileListDto updateFileListDto)
        {
            if (await _context.FileLists.FindAsync(id) is not FileList fileList)
            {
                return NotFound($"找不到此檔案-[{id}]");
            }
            else if (fileList.Path == updateFileListDto.Path)
            {
                return Forbidden($"已有此檔案路徑-[{updateFileListDto.Path}]");
            }
            fileList.Name = Path.GetFileName(updateFileListDto.Path);
            fileList.Path = updateFileListDto.Path;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileList(int id)
        {
            if (await _context.FileLists.FindAsync(id) is not FileList fileList)
            {
                return NotFound($"找不到此檔案-[{id}]");
            }
            fileList.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
