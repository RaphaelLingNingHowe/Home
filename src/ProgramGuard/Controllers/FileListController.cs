using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data.Dtos.FileList;
using ProgramGuard.Enums;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;
using ProgramGuard.Services;

namespace ProgramGuard.Controllers
{
    public class FileListController : BaseController
    {
        private readonly DigitalSignatureHelper _digitalSignatureHelper;
        private readonly FileListChangeNotifier _fileListChangeNotifier;
        public FileListController(ProgramGuardContext context, ILogger<BaseController> logger, DigitalSignatureHelper digitalSignatureHelper, FileListChangeNotifier fileListChangeNotifier) : base(context, logger)
        {
            _digitalSignatureHelper = digitalSignatureHelper;
            _fileListChangeNotifier = fileListChangeNotifier;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFileListsAsync()
        {
            if (!VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_FILELIST))
            {
                return Forbidden("權限不足，無法查看");
            }

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
        public async Task<IActionResult> CreateFileListAsync(CreateFileListDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.ADD_FILELIST))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (!System.IO.File.Exists(dto.Path))
            {
                return Forbidden("檔案不存在");
            }

            if (await _context.FileLists.AnyAsync(f => f.Path == dto.Path && !f.IsDeleted))
            {
                return Forbidden($"已有此檔案路徑-[{dto.Path}]");
            }

            FileList fileList = new()
            {
                Name = Path.GetFileName(dto.Path),
                Path = dto.Path
            };

            await _context.AddAsync(fileList);
            await _context.SaveChangesAsync();

            // TODO 應該在此直接給檔案路徑
            _fileListChangeNotifier.NotifyFileListChanged();
            await LogActionAsync(ACTION.CREATE_FILELIST, $"檔案路徑：{dto.Path}");

            ChangeLog changeLog = new()
            {
                FileListId = fileList.Id,
                DigitalSignature = _digitalSignatureHelper.VerifyDigitalSignature(dto.Path),
                Sha512 = FileHashHelper.ComputeSHA512Hash(dto.Path)
            };

            await _context.AddAsync(changeLog);
            await _context.SaveChangesAsync();
            return Created(fileList.Id);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateFileListAsync(int id, [FromBody] UpdateFileListDto dto)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_FILELIST))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (!System.IO.File.Exists(dto.Path))
            {
                return Forbidden("檔案不存在");
            }

            if (await _context.FileLists.FindAsync(id) is not FileList fileList)
            {
                return NotFound($"找不到此檔案-[{id}]");
            }
            else if (fileList.Path == dto.Path)
            {
                return Forbidden($"已有此檔案路徑-[{dto.Path}]");
            }

            fileList.Name = Path.GetFileName(dto.Path);
            fileList.Path = dto.Path;
            await _context.SaveChangesAsync();

            // TODO 應該在此處理異動
            _fileListChangeNotifier.NotifyFileListChanged();
            await LogActionAsync(ACTION.MODIFY_FILELIST, $"原路徑：{fileList.Path}，新路徑：{dto.Path}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileListAsync(int id)
        {
            if (!OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.DELETE_FILELIST))
            {
                return Forbidden("權限不足，無法執行操作");
            }

            if (await _context.FileLists.FindAsync(id) is not FileList fileList)
            {
                return NotFound($"找不到此檔案-[{id}]");
            }

            fileList.IsDeleted = true;
            await _context.SaveChangesAsync();

            // TODO 應該在此處理異動
            _fileListChangeNotifier.NotifyFileListChanged();
            await LogActionAsync(ACTION.DELETE_FILELIST, $"檔案路徑-[{fileList.Path}]");
            return NoContent();
        }
    }
}
