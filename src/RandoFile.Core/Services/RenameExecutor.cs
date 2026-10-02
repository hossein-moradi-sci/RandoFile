using System.Diagnostics;
using RandoFile.Core.Models;

namespace RandoFile.Core.Services;

/// <summary>
/// Runs a <see cref="RenamePlan"/> with a two-phase algorithm:
/// every file is first moved to a unique temporary name (which frees all original names),
/// then moved to its final name. This makes swaps and circular renames safe, and any failure
/// triggers an automatic rollback so the folder is never left half renamed.
/// </summary>
public sealed class RenameExecutor
{
    /// <summary>Prefix used for the staging files; the scanner ignores names starting with it.</summary>
    public const string TempFilePrefix = ".rf_tmp_";

    /// <summary>
    /// Executes the plan. Returns a failed result (without touching anything) when the plan contains
    /// conflicts, and rolls back automatically when a file cannot be renamed.
    /// </summary>
    public RenameResult Execute(
        RenamePlan plan,
        IProgress<RenameProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var stopwatch = Stopwatch.StartNew();

        if (plan.HasConflicts)
        {
            return new RenameResult
            {
                Success = false,
                Requested = plan.Items.Count,
                Errors = plan.Conflicts
                    .Select(conflict => new RenameError(conflict, "Conflicting target name."))
                    .ToList(),
                Duration = stopwatch.Elapsed,
            };
        }

        var pending = plan.Items.Where(item => !item.IsNoOp).ToList();
        if (pending.Count == 0)
        {
            return new RenameResult
            {
                Success = true,
                Requested = plan.Items.Count,
                Completed = 0,
                Duration = stopwatch.Elapsed,
            };
        }

        var errors = new List<RenameError>();
        var staged = new List<StagedFile>(pending.Count);

        // ---- Phase 1: move everything to a temporary, guaranteed unique name.
        try
        {
            for (var index = 0; index < pending.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = pending[index];
                if (!File.Exists(item.SourcePath))
                {
                    throw new FileNotFoundException("The file could not be found.", item.SourcePath);
                }

                var temp = CreateTempPath(item.SourcePath);
                File.Move(item.SourcePath, temp);
                staged.Add(new StagedFile(item, temp));

                progress?.Report(new RenameProgress(RenameStage.Staging, index + 1, pending.Count, item.SourceFileName));
            }
        }
        catch (Exception ex)
        {
            errors.Add(ToError(ex, pending, staged.Count));
            var rolledBack = RollbackStaging(staged, errors, progress);

            return new RenameResult
            {
                Success = false,
                Requested = plan.Items.Count,
                Completed = 0,
                RolledBack = rolledBack,
                Cancelled = ex is OperationCanceledException,
                Errors = errors,
                Duration = stopwatch.Elapsed,
            };
        }

        // ---- Phase 2: move every staged file to its final name.
        try
        {
            for (var index = 0; index < staged.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = staged[index];
                File.Move(entry.TempPath, entry.Item.TargetPath);
                entry.Committed = true;

                progress?.Report(new RenameProgress(RenameStage.Committing, index + 1, staged.Count, entry.Item.TargetFileName));
            }
        }
        catch (Exception ex)
        {
            errors.Add(ToError(ex, staged.Where(entry => !entry.Committed).Select(entry => entry.Item).ToList(), staged.Count));
            RollbackEverything(staged, errors, progress);

            return new RenameResult
            {
                Success = false,
                Requested = plan.Items.Count,
                Completed = 0,
                RolledBack = true,
                Cancelled = ex is OperationCanceledException,
                Errors = errors,
                Duration = stopwatch.Elapsed,
            };
        }

        progress?.Report(new RenameProgress(RenameStage.Committing, staged.Count, staged.Count));

        return new RenameResult
        {
            Success = true,
            Requested = plan.Items.Count,
            Completed = staged.Count,
            Errors = errors,
            Duration = stopwatch.Elapsed,
        };
    }

    /// <summary>Builds a path that is unique inside the folder and keeps the original extension.</summary>
    private static string CreateTempPath(string sourcePath)
    {
        var directory = Path.GetDirectoryName(sourcePath) ?? string.Empty;
        var extension = Path.GetExtension(sourcePath);

        for (var attempt = 0; attempt < 100; attempt++)
        {
            var candidate = Path.Combine(directory, $"{TempFilePrefix}{Guid.NewGuid():N}{extension}");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new IOException("Could not reserve a temporary file name.");
    }

    private static RenameError ToError(Exception exception, IReadOnlyList<RenameItem> items, int processedCount)
    {
        var name = items.Count > 0 && processedCount < items.Count
            ? items[processedCount].SourceFileName
            : items.LastOrDefault()?.SourceFileName ?? string.Empty;

        var message = exception switch
        {
            UnauthorizedAccessException => "Access to the file was denied.",
            IOException io => io.Message,
            OperationCanceledException => "The operation was cancelled.",
            _ => exception.Message,
        };

        return new RenameError(name, message);
    }

    /// <summary>Moves staged files back to their original names. Used when phase 1 fails.</summary>
    private static bool RollbackStaging(
        IReadOnlyList<StagedFile> staged,
        ICollection<RenameError> errors,
        IProgress<RenameProgress>? progress)
    {
        progress?.Report(new RenameProgress(RenameStage.RollingBack, 0, staged.Count));

        var restored = 0;
        for (var index = staged.Count - 1; index >= 0; index--)
        {
            var entry = staged[index];
            try
            {
                File.Move(entry.TempPath, entry.Item.SourcePath);
                restored++;
            }
            catch (Exception ex)
            {
                errors.Add(new RenameError(entry.Item.SourceFileName, $"Rollback failed: {ex.Message}"));
            }

            progress?.Report(new RenameProgress(RenameStage.RollingBack, staged.Count - index, staged.Count, entry.Item.SourceFileName));
        }

        return restored > 0;
    }

    /// <summary>
    /// Restores the folder after a phase 2 failure: finished files go back to their temporary name
    /// first (so no name is occupied twice), then every temporary file goes back to its original name.
    /// </summary>
    private static void RollbackEverything(
        IReadOnlyList<StagedFile> staged,
        ICollection<RenameError> errors,
        IProgress<RenameProgress>? progress)
    {
        progress?.Report(new RenameProgress(RenameStage.RollingBack, 0, staged.Count));

        for (var index = staged.Count - 1; index >= 0; index--)
        {
            var entry = staged[index];
            if (!entry.Committed)
            {
                continue;
            }

            try
            {
                File.Move(entry.Item.TargetPath, entry.TempPath);
                entry.Committed = false;
            }
            catch (Exception ex)
            {
                errors.Add(new RenameError(entry.Item.TargetFileName, $"Rollback failed: {ex.Message}"));
            }
        }

        RollbackStaging(staged.Where(entry => File.Exists(entry.TempPath)).ToList(), errors, progress);
    }

    /// <summary>Mutable bookkeeping for one file while it travels through the two phases.</summary>
    private sealed class StagedFile
    {
        public StagedFile(RenameItem item, string tempPath)
        {
            Item = item;
            TempPath = tempPath;
        }

        public RenameItem Item { get; }

        public string TempPath { get; }

        public bool Committed { get; set; }
    }
}
