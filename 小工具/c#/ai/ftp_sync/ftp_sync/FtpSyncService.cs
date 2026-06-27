using System.Collections.Concurrent;
using FluentFTP;

namespace ftp_sync;

public sealed record FtpSyncRequest(
    string Host,
    int Port,
    string Username,
    string Password,
    string LocalPath,
    string RemotePath,
    string SyncHistoryPath,
    bool UsePassiveMode,
    bool UseSsl,
    bool MirrorMode,
    bool DryRun,
    bool SyncEnvFiles,
    bool SyncVendorFiles,
    bool HighSpeedMode,
    int ParallelUploads,
    string ExcludePatterns);

public sealed record SyncProgress(
    string Status,
    string Message,
    int TotalFiles,
    int CompletedFiles,
    int ScannedFiles,
    int UploadedFiles,
    int SkippedFiles,
    int DeletedEntries,
    int FailedEntries);

public sealed record SyncResult(
    int ScannedFiles,
    int UploadedFiles,
    int SkippedFiles,
    int DeletedEntries,
    int FailedEntries,
    IReadOnlyList<SyncFailure> Failures);

public sealed record SyncFailure(
    string RelativePath,
    string ErrorMessage);

internal sealed record LocalSyncFile(
    string LocalPath,
    string RelativePath);

internal sealed record RemoteSyncFile(
    string RelativePath,
    string RemoteFilePath,
    DateTime RemoteWriteTimeUtc,
    long FileSize);

internal sealed record PendingUpload(
    string LocalPath,
    string RelativePath,
    string RemoteFilePath,
    string RemoteDirectoryPath,
    DateTime LocalWriteTimeUtc,
    long FileSize);

internal sealed record PendingDownload(
    string LocalPath,
    string RelativePath,
    string RemoteFilePath,
    DateTime RemoteWriteTimeUtc,
    long FileSize);

internal sealed record UploadExecutionResult(
    List<PendingUpload> SuccessfulUploads,
    List<SyncFailure> Failures);

internal sealed record DownloadExecutionResult(
    List<PendingDownload> SuccessfulDownloads,
    List<SyncFailure> Failures);

internal sealed class ProgressSnapshot
{
    public int CompletedFiles;
    public int UploadedFiles;
    public int FailedEntries;
}

internal sealed class PartitionBucket<TItem>
{
    public List<TItem> Items { get; } = [];

    public long TotalSize { get; set; }
}

public static class FtpSyncService
{
    private const string UploadSyncMode = "upload";
    private const string DownloadSyncMode = "download";
    private const int NormalProgressIntervalMs = 200;
    private const int HighSpeedProgressIntervalMs = 1000;

    public static async Task TestConnectionAsync(FtpSyncRequest request, CancellationToken cancellationToken)
    {
        await using var client = CreateClient(request);
        await client.Connect(cancellationToken);
        await client.GetWorkingDirectory(cancellationToken);
        await client.Disconnect(cancellationToken);
    }

    public static async Task<SyncResult> SyncAsync(FtpSyncRequest request, IProgress<SyncProgress>? progress, CancellationToken cancellationToken)
    {
        await using var client = CreateClient(request);
        await client.Connect(cancellationToken);

        var matcher = new ExcludeMatcher(request.ExcludePatterns);
        var localRoot = Path.GetFullPath(request.LocalPath);
        var remoteRoot = NormalizeRemotePath(request.RemotePath);
        var gitIgnoreMatcher = GitIgnoreMatcher.Load(localRoot);
        var history = SyncHistoryStore.Load(request.SyncHistoryPath);
        var profileKey = SyncHistoryStore.BuildProfileKey(request, UploadSyncMode);

        await client.CreateDirectory(remoteRoot, true, cancellationToken);

        var localFiles = CollectLocalFiles(localRoot, matcher, gitIgnoreMatcher, request.SyncEnvFiles, request.SyncVendorFiles);

        history.RemoveMissingEntries(profileKey, localFiles.Select(file => file.RelativePath).ToList());

        var scannedFiles = 0;
        var uploadedFiles = 0;
        var skippedFiles = 0;
        var deletedEntries = 0;
        var failedEntries = 0;
        var pendingUploads = new List<PendingUpload>();
        var failures = new List<SyncFailure>();
        var totalFiles = localFiles.Count;

        var expectedRemoteFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var expectedRemoteDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { remoteRoot };

        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("連線完成", $"套用排除規則與 .gitignore 後剩 {localFiles.Count} 個本機檔案。", totalFiles, 0, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));

        foreach (var localFile in localFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            scannedFiles++;

            var remoteFilePath = CombineRemotePath(remoteRoot, localFile.RelativePath);
            var remoteDirectoryPath = GetRemoteDirectoryPath(remoteFilePath);

            expectedRemoteFiles.Add(remoteFilePath);
            AddDirectoryTree(expectedRemoteDirectories, remoteRoot, localFile.RelativePath);

            ReportProgress(progress, request.HighSpeedMode, new SyncProgress("比對中", $"檢查 {localFile.RelativePath}", totalFiles, uploadedFiles + skippedFiles + failedEntries, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));

            var localInfo = new FileInfo(localFile.LocalPath);
            var shouldUpload = true;

            if (history.TryGetEntry(profileKey, localFile.RelativePath, out var historyEntry))
            {
                shouldUpload = localInfo.LastWriteTimeUtc > historyEntry.LocalWriteTimeUtc ||
                    localInfo.Length != historyEntry.FileSize;
            }

            if (!shouldUpload)
            {
                skippedFiles++;
                ReportProgress(progress, request.HighSpeedMode, new SyncProgress("略過", $"略過 {localFile.RelativePath} (本地紀錄未變更)", totalFiles, uploadedFiles + skippedFiles + failedEntries, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));
                continue;
            }

            if (request.DryRun)
            {
                uploadedFiles++;
                ReportProgress(progress, request.HighSpeedMode, new SyncProgress("預演", $"將上傳 {localFile.RelativePath}", totalFiles, uploadedFiles + skippedFiles + failedEntries, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));
            }
            else
            {
                pendingUploads.Add(new PendingUpload(
                    localFile.LocalPath,
                    localFile.RelativePath,
                    remoteFilePath,
                    remoteDirectoryPath,
                    localInfo.LastWriteTimeUtc,
                    localInfo.Length));
            }
        }

        if (!request.DryRun && pendingUploads.Count > 0)
        {
            var uploadResult = await UploadFilesInParallelAsync(
                request,
                pendingUploads,
                scannedFiles,
                skippedFiles,
                uploadedFiles,
                deletedEntries,
                failedEntries,
                totalFiles,
                progress,
                cancellationToken);

            uploadedFiles += uploadResult.SuccessfulUploads.Count;
            failedEntries += uploadResult.Failures.Count;
            failures.AddRange(uploadResult.Failures);

            foreach (var upload in uploadResult.SuccessfulUploads)
            {
                history.SetEntry(profileKey, upload.RelativePath, upload.LocalWriteTimeUtc, upload.FileSize);
                if (history.TryGetEntry(profileKey, upload.RelativePath, out var updatedEntry))
                {
                    updatedEntry.RemoteWriteTimeUtc = upload.LocalWriteTimeUtc;
                }
            }
        }

        if (request.MirrorMode)
        {
            deletedEntries = await DeleteRemoteExtrasAsync(
                client,
                request.DryRun,
                remoteRoot,
                expectedRemoteFiles,
                expectedRemoteDirectories,
                matcher,
                gitIgnoreMatcher,
                request.SyncEnvFiles,
                request.SyncVendorFiles,
                scannedFiles,
                uploadedFiles,
                skippedFiles,
                deletedEntries,
                failedEntries,
                totalFiles,
                progress,
                cancellationToken);
        }

        if (!request.DryRun)
        {
            history.Save();
        }

        await client.Disconnect(cancellationToken);
        return new SyncResult(scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries, failures);
    }

    public static async Task<SyncResult> ReverseSyncAsync(FtpSyncRequest request, IProgress<SyncProgress>? progress, CancellationToken cancellationToken)
    {
        await using var client = CreateClient(request);
        await client.Connect(cancellationToken);

        var matcher = new ExcludeMatcher(request.ExcludePatterns);
        var localRoot = Path.GetFullPath(request.LocalPath);
        var remoteRoot = NormalizeRemotePath(request.RemotePath);
        var gitIgnoreMatcher = GitIgnoreMatcher.Load(localRoot);
        var history = SyncHistoryStore.Load(request.SyncHistoryPath);
        var profileKey = SyncHistoryStore.BuildProfileKey(request, DownloadSyncMode);

        Directory.CreateDirectory(localRoot);

        var remoteFiles = await CollectRemoteFilesAsync(
            client,
            remoteRoot,
            matcher,
            gitIgnoreMatcher,
            request.SyncEnvFiles,
            request.SyncVendorFiles,
            cancellationToken);

        history.RemoveMissingEntries(profileKey, remoteFiles.Select(file => file.RelativePath).ToList());

        var scannedFiles = 0;
        var downloadedFiles = 0;
        var skippedFiles = 0;
        var deletedEntries = 0;
        var failedEntries = 0;
        var pendingDownloads = new List<PendingDownload>();
        var failures = new List<SyncFailure>();
        var totalFiles = remoteFiles.Count;
        var expectedLocalFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("連線完成", $"套用排除規則與 .gitignore 後剩 {remoteFiles.Count} 個遠端檔案。", totalFiles, 0, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));

        foreach (var remoteFile in remoteFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            scannedFiles++;
            expectedLocalFiles.Add(remoteFile.RelativePath);

            ReportProgress(progress, request.HighSpeedMode, new SyncProgress("比對中", $"檢查 {remoteFile.RelativePath}", totalFiles, downloadedFiles + skippedFiles + failedEntries, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));

            var localFilePath = Path.Combine(localRoot, remoteFile.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            var shouldDownload = true;

            if (history.TryGetEntry(profileKey, remoteFile.RelativePath, out var historyEntry))
            {
                shouldDownload = remoteFile.RemoteWriteTimeUtc > (historyEntry.RemoteWriteTimeUtc ?? DateTime.MinValue) ||
                    remoteFile.FileSize != historyEntry.FileSize ||
                    !File.Exists(localFilePath);
            }

            if (!shouldDownload)
            {
                skippedFiles++;
                ReportProgress(progress, request.HighSpeedMode, new SyncProgress("略過", $"略過 {remoteFile.RelativePath} (遠端紀錄未變更)", totalFiles, downloadedFiles + skippedFiles + failedEntries, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));
                continue;
            }

            if (request.DryRun)
            {
                downloadedFiles++;
                ReportProgress(progress, request.HighSpeedMode, new SyncProgress("預演", $"將下載 {remoteFile.RelativePath}", totalFiles, downloadedFiles + skippedFiles + failedEntries, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));
            }
            else
            {
                pendingDownloads.Add(new PendingDownload(
                    localFilePath,
                    remoteFile.RelativePath,
                    remoteFile.RemoteFilePath,
                    remoteFile.RemoteWriteTimeUtc,
                    remoteFile.FileSize));
            }
        }

        if (!request.DryRun && pendingDownloads.Count > 0)
        {
            var downloadResult = await DownloadFilesInParallelAsync(
                request,
                pendingDownloads,
                scannedFiles,
                skippedFiles,
                downloadedFiles,
                deletedEntries,
                failedEntries,
                totalFiles,
                progress,
                cancellationToken);

            downloadedFiles += downloadResult.SuccessfulDownloads.Count;
            failedEntries += downloadResult.Failures.Count;
            failures.AddRange(downloadResult.Failures);

            foreach (var download in downloadResult.SuccessfulDownloads)
            {
                history.SetEntry(profileKey, download.RelativePath, download.RemoteWriteTimeUtc, download.FileSize);
                if (history.TryGetEntry(profileKey, download.RelativePath, out var updatedEntry))
                {
                    updatedEntry.RemoteWriteTimeUtc = download.RemoteWriteTimeUtc;
                }
            }
        }

        if (request.MirrorMode)
        {
            deletedEntries = DeleteLocalExtras(
                request.DryRun,
                localRoot,
                expectedLocalFiles,
                matcher,
                gitIgnoreMatcher,
                request.SyncEnvFiles,
                request.SyncVendorFiles,
                scannedFiles,
                downloadedFiles,
                skippedFiles,
                deletedEntries,
                failedEntries,
                totalFiles,
                progress,
                cancellationToken);
        }

        if (!request.DryRun)
        {
            history.Save();
        }

        await client.Disconnect(cancellationToken);
        return new SyncResult(scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries, failures);
    }

    private static async Task<UploadExecutionResult> UploadFilesInParallelAsync(
        FtpSyncRequest request,
        IReadOnlyCollection<PendingUpload> pendingUploads,
        int scannedFiles,
        int skippedFiles,
        int uploadedFiles,
        int deletedEntries,
        int failedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        var results = new ConcurrentBag<PendingUpload>();
        var failures = new ConcurrentBag<SyncFailure>();
        var snapshot = new ProgressSnapshot
        {
            UploadedFiles = uploadedFiles,
            FailedEntries = failedEntries,
            CompletedFiles = uploadedFiles + skippedFiles + failedEntries
        };

        var workerCount = Math.Max(1, Math.Min(request.ParallelUploads, pendingUploads.Count));
        var partitions = BuildBalancedPartitions(pendingUploads, workerCount, upload => upload.FileSize);

        await Task.WhenAll(partitions.Select(partition => ProcessUploadPartitionAsync(
            request,
            partition,
            results,
            failures,
            snapshot,
            scannedFiles,
            skippedFiles,
            deletedEntries,
            totalFiles,
            progress,
            cancellationToken)));

        return new UploadExecutionResult(results.ToList(), failures.ToList());
    }

    private static async Task<DownloadExecutionResult> DownloadFilesInParallelAsync(
        FtpSyncRequest request,
        IReadOnlyCollection<PendingDownload> pendingDownloads,
        int scannedFiles,
        int skippedFiles,
        int downloadedFiles,
        int deletedEntries,
        int failedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        var results = new ConcurrentBag<PendingDownload>();
        var failures = new ConcurrentBag<SyncFailure>();
        var snapshot = new ProgressSnapshot
        {
            UploadedFiles = downloadedFiles,
            FailedEntries = failedEntries,
            CompletedFiles = downloadedFiles + skippedFiles + failedEntries
        };

        var workerCount = Math.Max(1, Math.Min(request.ParallelUploads, pendingDownloads.Count));
        var partitions = BuildBalancedPartitions(pendingDownloads, workerCount, download => download.FileSize);

        await Task.WhenAll(partitions.Select(partition => ProcessDownloadPartitionAsync(
            request,
            partition,
            results,
            failures,
            snapshot,
            scannedFiles,
            skippedFiles,
            deletedEntries,
            totalFiles,
            progress,
            cancellationToken)));

        return new DownloadExecutionResult(results.ToList(), failures.ToList());
    }

    private static async Task ProcessUploadPartitionAsync(
        FtpSyncRequest request,
        IReadOnlyCollection<PendingUpload> uploads,
        ConcurrentBag<PendingUpload> results,
        ConcurrentBag<SyncFailure> failures,
        ProgressSnapshot snapshot,
        int scannedFiles,
        int skippedFiles,
        int deletedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (uploads.Count == 0)
        {
            return;
        }

        await using var uploadClient = CreateClient(request);
        await uploadClient.Connect(cancellationToken);
        var ensuredDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var uploadGroups = uploads
            .GroupBy(upload => upload.RemoteDirectoryPath, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(group => group.Sum(item => item.FileSize))
            .ToList();

        foreach (var uploadGroup in uploadGroups)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (ensuredDirectories.Add(uploadGroup.Key))
                {
                    await uploadClient.CreateDirectory(uploadGroup.Key, true, cancellationToken);
                }

                long lastProgressTick = 0;
                var transferProgress = new Progress<FtpProgress>(ftpProgress =>
                {
                    var now = Environment.TickCount64;
                    if (now - lastProgressTick < (request.HighSpeedMode ? HighSpeedProgressIntervalMs : NormalProgressIntervalMs))
                    {
                        return;
                    }

                    lastProgressTick = now;
                    var percent = ftpProgress.Progress < 0 ? "?" : ftpProgress.Progress.ToString("0.0");
                    var speed = ftpProgress.TransferSpeed <= 0 ? "-" : $"{ftpProgress.TransferSpeed / 1024d:0.0} KB/s";
                    ReportProgress(progress, request.HighSpeedMode, new SyncProgress(
                        "上傳中",
                        $"批量上傳 {uploadGroup.Key} ({percent}%, {speed})",
                        totalFiles,
                        Volatile.Read(ref snapshot.CompletedFiles),
                        scannedFiles,
                        Volatile.Read(ref snapshot.UploadedFiles),
                        skippedFiles,
                        deletedEntries,
                        Volatile.Read(ref snapshot.FailedEntries)));
                });

                var batchResults = await uploadClient.UploadFiles(
                    uploadGroup.Select(item => item.LocalPath),
                    uploadGroup.Key,
                    FtpRemoteExists.Resume,
                    false,
                    FtpVerify.None,
                    FtpError.None,
                    cancellationToken,
                    transferProgress);

                var uploadLookup = uploadGroup.ToDictionary(item => item.LocalPath, StringComparer.OrdinalIgnoreCase);

                foreach (var batchResult in batchResults)
                {
                    if (!uploadLookup.TryGetValue(batchResult.LocalPath, out var upload))
                    {
                        continue;
                    }

                    if (batchResult.IsSuccess && !batchResult.IsFailed)
                    {
                        Interlocked.Increment(ref snapshot.UploadedFiles);
                        var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                        results.Add(upload);
                        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("完成", $"已上傳 {upload.RelativePath}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                    }
                    else
                    {
                        var errorMessage = batchResult.Exception?.Message ?? "批量上傳失敗";
                        Interlocked.Increment(ref snapshot.FailedEntries);
                        var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                        failures.Add(new SyncFailure(upload.RelativePath, errorMessage));
                        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("失敗", $"上傳失敗 {upload.RelativePath}: {errorMessage}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                foreach (var upload in uploadGroup)
                {
                    Interlocked.Increment(ref snapshot.FailedEntries);
                    var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                    failures.Add(new SyncFailure(upload.RelativePath, ex.Message));
                    ReportProgress(progress, request.HighSpeedMode, new SyncProgress("失敗", $"上傳失敗 {upload.RelativePath}: {ex.Message}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                }
            }
        }

        await uploadClient.Disconnect(cancellationToken);
    }

    private static async Task ProcessDownloadPartitionAsync(
        FtpSyncRequest request,
        IReadOnlyCollection<PendingDownload> downloads,
        ConcurrentBag<PendingDownload> results,
        ConcurrentBag<SyncFailure> failures,
        ProgressSnapshot snapshot,
        int scannedFiles,
        int skippedFiles,
        int deletedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (downloads.Count == 0)
        {
            return;
        }

        await using var downloadClient = CreateClient(request);
        await downloadClient.Connect(cancellationToken);
        var downloadGroups = downloads
            .GroupBy(download => Path.GetDirectoryName(download.LocalPath) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(group => group.Sum(item => item.FileSize))
            .ToList();

        foreach (var downloadGroup in downloadGroups)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Directory.CreateDirectory(downloadGroup.Key);
                long lastProgressTick = 0;
                var transferProgress = new Progress<FtpProgress>(ftpProgress =>
                {
                    var now = Environment.TickCount64;
                    if (now - lastProgressTick < (request.HighSpeedMode ? HighSpeedProgressIntervalMs : NormalProgressIntervalMs))
                    {
                        return;
                    }

                    lastProgressTick = now;
                    var percent = ftpProgress.Progress < 0 ? "?" : ftpProgress.Progress.ToString("0.0");
                    var speed = ftpProgress.TransferSpeed <= 0 ? "-" : $"{ftpProgress.TransferSpeed / 1024d:0.0} KB/s";
                    ReportProgress(progress, request.HighSpeedMode, new SyncProgress(
                        "下載中",
                        $"批量下載 {downloadGroup.Key} ({percent}%, {speed})",
                        totalFiles,
                        Volatile.Read(ref snapshot.CompletedFiles),
                        scannedFiles,
                        Volatile.Read(ref snapshot.UploadedFiles),
                        skippedFiles,
                        deletedEntries,
                        Volatile.Read(ref snapshot.FailedEntries)));
                });

                var batchResults = await downloadClient.DownloadFiles(
                    downloadGroup.Key,
                    downloadGroup.Select(item => item.RemoteFilePath),
                    FtpLocalExists.Resume,
                    FtpVerify.None,
                    FtpError.None,
                    cancellationToken,
                    transferProgress);

                var downloadLookup = downloadGroup.ToDictionary(item => item.RemoteFilePath, StringComparer.OrdinalIgnoreCase);

                foreach (var batchResult in batchResults)
                {
                    if (!downloadLookup.TryGetValue(NormalizeRemotePath(batchResult.RemotePath), out var download))
                    {
                        continue;
                    }

                    if (batchResult.IsSuccess && !batchResult.IsFailed)
                    {
                        File.SetLastWriteTimeUtc(download.LocalPath, download.RemoteWriteTimeUtc);
                        Interlocked.Increment(ref snapshot.UploadedFiles);
                        var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                        results.Add(download);
                        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("完成", $"已下載 {download.RelativePath}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                    }
                    else
                    {
                        var errorMessage = batchResult.Exception?.Message ?? "批量下載失敗";
                        Interlocked.Increment(ref snapshot.FailedEntries);
                        var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                        failures.Add(new SyncFailure(download.RelativePath, errorMessage));
                        ReportProgress(progress, request.HighSpeedMode, new SyncProgress("失敗", $"下載失敗 {download.RelativePath}: {errorMessage}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                foreach (var download in downloadGroup)
                {
                    Interlocked.Increment(ref snapshot.FailedEntries);
                    var currentCompleted = Interlocked.Increment(ref snapshot.CompletedFiles);
                    failures.Add(new SyncFailure(download.RelativePath, ex.Message));
                    ReportProgress(progress, request.HighSpeedMode, new SyncProgress("失敗", $"下載失敗 {download.RelativePath}: {ex.Message}", totalFiles, currentCompleted, scannedFiles, Volatile.Read(ref snapshot.UploadedFiles), skippedFiles, deletedEntries, Volatile.Read(ref snapshot.FailedEntries)));
                }
            }
        }

        await downloadClient.Disconnect(cancellationToken);
    }

    private static async Task<int> DeleteRemoteExtrasAsync(
        AsyncFtpClient client,
        bool dryRun,
        string remoteRoot,
        HashSet<string> expectedRemoteFiles,
        HashSet<string> expectedRemoteDirectories,
        ExcludeMatcher matcher,
        GitIgnoreMatcher gitIgnoreMatcher,
        bool syncEnvFiles,
        bool syncVendorFiles,
        int scannedFiles,
        int uploadedFiles,
        int skippedFiles,
        int deletedEntries,
        int failedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        var remoteItems = await client.GetListing(remoteRoot, FtpListOption.Recursive, cancellationToken);
        var orderedItems = remoteItems
            .OrderByDescending(item => item.FullName.Count(ch => ch == '/'))
            .ToList();

        foreach (var item in orderedItems)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var relativePath = ToRelativeRemotePath(remoteRoot, item.FullName);
            var isDirectory = item.Type == FtpObjectType.Directory;

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                continue;
            }

            if (matcher.IsExcluded(relativePath, isDirectory) || IsIgnoredByGitIgnore(relativePath, isDirectory, syncEnvFiles, syncVendorFiles, gitIgnoreMatcher))
            {
                continue;
            }

            if (item.Type == FtpObjectType.File)
            {
                if (expectedRemoteFiles.Contains(NormalizeRemotePath(item.FullName)))
                {
                    continue;
                }

                deletedEntries++;
                if (!dryRun)
                {
                    await client.DeleteFile(item.FullName, cancellationToken);
                }

                progress?.Report(new SyncProgress(dryRun ? "預演" : "鏡像清理", $"{(dryRun ? "將刪除" : "刪除")}遠端檔案 {relativePath}", totalFiles, uploadedFiles + skippedFiles + failedEntries, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));
                continue;
            }

            if (item.Type == FtpObjectType.Directory)
            {
                var normalizedDirectory = NormalizeRemotePath(item.FullName);
                if (expectedRemoteDirectories.Contains(normalizedDirectory))
                {
                    continue;
                }

                if (expectedRemoteFiles.Any(path => path.StartsWith(normalizedDirectory + "/", StringComparison.OrdinalIgnoreCase)) ||
                    expectedRemoteDirectories.Any(path => path.StartsWith(normalizedDirectory + "/", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                deletedEntries++;
                if (!dryRun)
                {
                    await client.DeleteDirectory(item.FullName, cancellationToken);
                }

                progress?.Report(new SyncProgress(dryRun ? "預演" : "鏡像清理", $"{(dryRun ? "將刪除" : "刪除")}遠端資料夾 {relativePath}", totalFiles, uploadedFiles + skippedFiles + failedEntries, scannedFiles, uploadedFiles, skippedFiles, deletedEntries, failedEntries));
            }
        }

        return deletedEntries;
    }

    private static void AddDirectoryTree(HashSet<string> directories, string remoteRoot, string relativePath)
    {
        var normalizedRelative = NormalizeRelativePath(relativePath);
        var segments = normalizedRelative.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = remoteRoot.TrimEnd('/');

        for (var i = 0; i < segments.Length - 1; i++)
        {
            current += "/" + segments[i];
            directories.Add(current);
        }
    }

    private static async Task<List<RemoteSyncFile>> CollectRemoteFilesAsync(
        AsyncFtpClient client,
        string remoteRoot,
        ExcludeMatcher matcher,
        GitIgnoreMatcher gitIgnoreMatcher,
        bool syncEnvFiles,
        bool syncVendorFiles,
        CancellationToken cancellationToken)
    {
        var remoteItems = await client.GetListing(remoteRoot, FtpListOption.Recursive, cancellationToken);
        var results = new List<RemoteSyncFile>();

        foreach (var item in remoteItems)
        {
            if (item.Type != FtpObjectType.File)
            {
                continue;
            }

            var relativePath = ToRelativeRemotePath(remoteRoot, item.FullName);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                continue;
            }

            if (matcher.IsExcluded(relativePath, isDirectory: false))
            {
                continue;
            }

            if (!ShouldIncludeFile(relativePath, syncEnvFiles, syncVendorFiles, gitIgnoreMatcher))
            {
                continue;
            }

            results.Add(new RemoteSyncFile(
                relativePath,
                NormalizeRemotePath(item.FullName),
                item.Modified.ToUniversalTime(),
                item.Size));
        }

        return results;
    }

    private static int DeleteLocalExtras(
        bool dryRun,
        string localRoot,
        HashSet<string> expectedLocalFiles,
        ExcludeMatcher matcher,
        GitIgnoreMatcher gitIgnoreMatcher,
        bool syncEnvFiles,
        bool syncVendorFiles,
        int scannedFiles,
        int downloadedFiles,
        int skippedFiles,
        int deletedEntries,
        int failedEntries,
        int totalFiles,
        IProgress<SyncProgress>? progress,
        CancellationToken cancellationToken)
    {
        var localFiles = CollectLocalFiles(localRoot, matcher, gitIgnoreMatcher, syncEnvFiles, syncVendorFiles);
        foreach (var localFile in localFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (expectedLocalFiles.Contains(localFile.RelativePath))
            {
                continue;
            }

            deletedEntries++;
            if (!dryRun)
            {
                File.Delete(localFile.LocalPath);
            }

            progress?.Report(new SyncProgress(dryRun ? "預演" : "鏡像清理", $"{(dryRun ? "將刪除" : "刪除")}本機檔案 {localFile.RelativePath}", totalFiles, downloadedFiles + skippedFiles + failedEntries, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));
        }

        var allDirectories = Directory.EnumerateDirectories(localRoot, "*", SearchOption.AllDirectories)
            .OrderByDescending(path => path.Count(ch => ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar));

        foreach (var directoryPath in allDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var relativePath = NormalizeRelativePath(Path.GetRelativePath(localRoot, directoryPath));
            if (string.IsNullOrWhiteSpace(relativePath) ||
                ShouldSkipDirectory(relativePath, matcher, gitIgnoreMatcher, syncEnvFiles, syncVendorFiles))
            {
                continue;
            }

            if (Directory.EnumerateFileSystemEntries(directoryPath).Any())
            {
                continue;
            }

            deletedEntries++;
            if (!dryRun)
            {
                Directory.Delete(directoryPath, false);
            }

            progress?.Report(new SyncProgress(dryRun ? "預演" : "鏡像清理", $"{(dryRun ? "將刪除" : "刪除")}本機資料夾 {relativePath}", totalFiles, downloadedFiles + skippedFiles + failedEntries, scannedFiles, downloadedFiles, skippedFiles, deletedEntries, failedEntries));
        }

        return deletedEntries;
    }

    private static AsyncFtpClient CreateClient(FtpSyncRequest request)
    {
        var client = new AsyncFtpClient(request.Host, request.Username, request.Password, request.Port);
        client.Config.DataConnectionType = request.UsePassiveMode
            ? FtpDataConnectionType.AutoPassive
            : FtpDataConnectionType.AutoActive;
        client.Config.EncryptionMode = request.UseSsl
            ? FtpEncryptionMode.Auto
            : FtpEncryptionMode.None;
        client.Config.ConnectTimeout = 10000;
        client.Config.ReadTimeout = 10000;

        return client;
    }

    private static void ReportProgress(IProgress<SyncProgress>? progress, bool highSpeedMode, SyncProgress snapshot)
    {
        if (progress is null)
        {
            return;
        }

        if (highSpeedMode && snapshot.Status is "比對中" or "略過")
        {
            return;
        }

        progress.Report(snapshot);
    }

    private static List<List<TItem>> BuildBalancedPartitions<TItem>(
        IReadOnlyCollection<TItem> items,
        int workerCount,
        Func<TItem, long> sizeSelector)
    {
        var partitions = Enumerable.Range(0, workerCount)
            .Select(_ => new PartitionBucket<TItem>())
            .ToList();

        foreach (var item in items.OrderByDescending(sizeSelector))
        {
            var target = partitions.MinBy(bucket => bucket.TotalSize)!;
            target.Items.Add(item);
            target.TotalSize += Math.Max(1, sizeSelector(item));
        }

        return partitions
            .Where(bucket => bucket.Items.Count > 0)
            .Select(bucket => bucket.Items)
            .ToList();
    }

    private static List<LocalSyncFile> CollectLocalFiles(
        string localRoot,
        ExcludeMatcher matcher,
        GitIgnoreMatcher gitIgnoreMatcher,
        bool syncEnvFiles,
        bool syncVendorFiles)
    {
        var results = new List<LocalSyncFile>();
        var pendingDirectories = new Stack<string>();
        var enumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            ReturnSpecialDirectories = false
        };

        pendingDirectories.Push(localRoot);

        while (pendingDirectories.Count > 0)
        {
            var currentDirectory = pendingDirectories.Pop();

            foreach (var directoryPath in Directory.EnumerateDirectories(currentDirectory, "*", enumerationOptions))
            {
                var relativeDirectory = NormalizeRelativePath(Path.GetRelativePath(localRoot, directoryPath));
                if (ShouldSkipDirectory(relativeDirectory, matcher, gitIgnoreMatcher, syncEnvFiles, syncVendorFiles))
                {
                    continue;
                }

                pendingDirectories.Push(directoryPath);
            }

            foreach (var filePath in Directory.EnumerateFiles(currentDirectory, "*", enumerationOptions))
            {
                var relativeFile = NormalizeRelativePath(Path.GetRelativePath(localRoot, filePath));
                if (matcher.IsExcluded(relativeFile, isDirectory: false))
                {
                    continue;
                }

                if (!ShouldIncludeFile(relativeFile, syncEnvFiles, syncVendorFiles, gitIgnoreMatcher))
                {
                    continue;
                }

                results.Add(new LocalSyncFile(filePath, relativeFile));
            }
        }

        return results;
    }

    private static bool ShouldSkipDirectory(
        string relativePath,
        ExcludeMatcher matcher,
        GitIgnoreMatcher gitIgnoreMatcher,
        bool syncEnvFiles,
        bool syncVendorFiles)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return false;
        }

        if (syncVendorFiles && IsVendorPath(relativePath))
        {
            return false;
        }

        if (matcher.IsExcluded(relativePath, isDirectory: true))
        {
            return true;
        }

        if (gitIgnoreMatcher.HasNegatedRules)
        {
            return false;
        }

        return IsIgnoredByGitIgnore(relativePath, isDirectory: true, syncEnvFiles, syncVendorFiles, gitIgnoreMatcher);
    }

    private static bool ShouldIncludeFile(string relativePath, bool syncEnvFiles, bool syncVendorFiles, GitIgnoreMatcher gitIgnoreMatcher)
    {
        if (syncEnvFiles && IsEnvFile(relativePath))
        {
            return true;
        }

        if (syncVendorFiles && IsVendorPath(relativePath))
        {
            return true;
        }

        return !gitIgnoreMatcher.IsIgnored(relativePath, isDirectory: false);
    }

    private static bool IsIgnoredByGitIgnore(string relativePath, bool isDirectory, bool syncEnvFiles, bool syncVendorFiles, GitIgnoreMatcher gitIgnoreMatcher)
    {
        if (!isDirectory && syncEnvFiles && IsEnvFile(relativePath))
        {
            return false;
        }

        if (syncVendorFiles && IsVendorPath(relativePath))
        {
            return false;
        }

        return gitIgnoreMatcher.IsIgnored(relativePath, isDirectory);
    }

    private static bool IsEnvFile(string relativePath)
    {
        var fileName = Path.GetFileName(relativePath.Replace('/', Path.DirectorySeparatorChar));
        return fileName.StartsWith(".env", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsVendorPath(string relativePath)
    {
        var normalized = NormalizeRelativePath(relativePath);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Any(segment => segment.Equals("vendor", StringComparison.OrdinalIgnoreCase));
    }

    private static string CombineRemotePath(string basePath, string relativeFilePath)
    {
        var normalizedBase = NormalizeRemotePath(basePath).TrimEnd('/');
        var normalizedRelative = NormalizeRelativePath(relativeFilePath).TrimStart('/');
        return string.IsNullOrWhiteSpace(normalizedBase)
            ? "/" + normalizedRelative
            : $"{normalizedBase}/{normalizedRelative}";
    }

    private static string GetRemoteDirectoryPath(string remoteFilePath)
    {
        var normalized = NormalizeRemotePath(remoteFilePath);
        var index = normalized.LastIndexOf('/');
        return index <= 0 ? "/" : normalized[..index];
    }

    private static string ToRelativeRemotePath(string remoteRoot, string fullRemotePath)
    {
        var normalizedRoot = NormalizeRemotePath(remoteRoot).TrimEnd('/');
        var normalizedFullPath = NormalizeRemotePath(fullRemotePath);

        if (normalizedFullPath.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        if (!normalizedFullPath.StartsWith(normalizedRoot + "/", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedFullPath.TrimStart('/');
        }

        return normalizedFullPath[(normalizedRoot.Length + 1)..];
    }

    private static string NormalizeRelativePath(string path)
    {
        var normalized = path.Replace('\\', '/').Trim().Trim('/');
        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        return normalized;
    }

    private static string NormalizeRemotePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/";
        }

        var normalized = path.Replace('\\', '/').Trim();
        if (!normalized.StartsWith('/'))
        {
            normalized = "/" + normalized;
        }

        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        return normalized.TrimEnd('/').Length == 0 ? "/" : normalized.TrimEnd('/');
    }
}
