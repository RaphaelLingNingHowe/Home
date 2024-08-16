using Microsoft.EntityFrameworkCore;
using ProgramGuard.Helper;
using ProgramGuard.Repository.Data;
using ProgramGuard.Repository.Models;
using ProgramGuard.Services;
using System.Collections.Concurrent;

namespace ProgramGuard.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly FileListChangeNotifier _fileListChangeNotifier;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new();

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, FileListChangeNotifier fileListChangeNotifier)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _fileListChangeNotifier = fileListChangeNotifier;

            // TODO 應該拆成更高效的方式, add / update / delete
            // 目前的作法當 event 發生時會重讀資料庫, 並重新判斷整份 filePath,
            // 但實際上 event 發生前就已經有變動的資料, 例如新增或是刪除, 直接把路徑傳進來即可
            _fileListChangeNotifier.FileListChanged += async (sender, args) => await RefreshWatchersAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker 執行時間： {time}", DateTime.Now);
            await InitializeWatchersAsync(stoppingToken);
        }

        private async Task RefreshWatchersAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("正在刷新 FileSystemWatcher...");
            await InitializeWatchersAsync(stoppingToken);
        }

        private async Task InitializeWatchersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProgramGuardContext>();
            var fileList = await context.FileLists
                    .Where(f => !f.IsDeleted)
                    .ToListAsync(stoppingToken);

            var currentPaths = new HashSet<string>(fileList.Select(f => f.Path));

            // 移除不再監視的文件
            foreach (var path in _watchers.Keys)
            {
                if (!currentPaths.Contains(path))
                {
                    if (_watchers.TryRemove(path, out var watcher))
                    {
                        watcher.Dispose();
                        _logger.LogInformation("停止監視檔案: {path}", path);
                    }
                }
            }

            // 添加新的監視器或更新現有的
            foreach (var file in fileList)
            {
                string filePath = file.Path;
                string? directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);
                if (directory == null)
                {
                    _logger.LogWarning("無法監視檔案，因為找不到目錄: {filePath}", filePath);
                    continue;
                }

                if (!_watchers.ContainsKey(filePath))
                {
                    var watcher = new FileSystemWatcher(directory)
                    {
                        Filter = fileName,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.Attributes,
                        EnableRaisingEvents = true
                    };
                    watcher.Changed += OnChangedAsync;

                    _watchers[filePath] = watcher;
                    _logger.LogInformation("開始監視檔案: {filePath}", filePath);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.Dispose();
            }
            _watchers.Clear();

            return base.StopAsync(cancellationToken);
        }

        private async void OnChangedAsync(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("檔案異動: {e.FullPath}", e.FullPath);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProgramGuardContext>();
            var digitalSignatureHelper = scope.ServiceProvider.GetRequiredService<DigitalSignatureHelper>();

            try
            {
                if (await context.ChangeLogs.Where(c => c.FileList.Path == e.FullPath).OrderByDescending(c => c.Timestamp).FirstOrDefaultAsync() is not ChangeLog lastChangeLog)
                {
                    _logger.LogWarning("找不到此檔案路徑-[{e.FullPath}]", e.FullPath);
                    return;
                }

                var sha512 = FileHashHelper.ComputeSHA512Hash(e.FullPath);
                if (lastChangeLog.Sha512 == sha512)
                {
                    _logger.LogInformation("檔案內容無異動-[{e.FullPath}]", e.FullPath);
                    return;
                }
                else
                {
                    ChangeLog changeLog = new()
                    {
                        FileListId = lastChangeLog.FileListId,
                        DigitalSignature = digitalSignatureHelper.VerifyDigitalSignature(e.FullPath),
                        Sha512 = sha512
                    };

                    await context.AddAsync(changeLog);
                    await context.SaveChangesAsync();
                }
                _logger.LogInformation("成功記錄異動檔案: {e.FullPath}", e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理異動檔案時發生錯誤: {e.FullPath}", e.FullPath);
            }
        }
    }
}
