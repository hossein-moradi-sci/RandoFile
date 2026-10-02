using RandoFile.Core.Abstractions;
using RandoFile.Core.Models;

namespace RandoFile.Core.Services;

/// <summary>
/// Turns a list of files plus a <see cref="NamingPattern"/> into a safe <see cref="RenamePlan"/>:
/// the order is shuffled, every original extension is preserved and duplicates/overwrites are detected.
/// </summary>
public sealed class RenamePlanBuilder
{
    private readonly IFileShuffler _shuffler;

    public RenamePlanBuilder(IFileShuffler? shuffler = null)
    {
        _shuffler = shuffler ?? new FileShuffler();
    }

    /// <summary>
    /// Builds the plan. When <paramref name="shuffle"/> is false the files keep their current order,
    /// which is useful for a deterministic preview and for tests.
    /// </summary>
    public RenamePlan Build(IReadOnlyList<string> sourceFiles, NamingPattern pattern, bool shuffle = true)
    {
        ArgumentNullException.ThrowIfNull(sourceFiles);
        ArgumentNullException.ThrowIfNull(pattern);

        var sources = sourceFiles
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var ordered = shuffle ? _shuffler.Shuffle(sources) : sources;
        var folder = sources.Count > 0 ? Path.GetDirectoryName(sources[0]) ?? string.Empty : string.Empty;

        var items = new List<RenameItem>(ordered.Count);
        for (var index = 0; index < ordered.Count; index++)
        {
            var source = ordered[index];
            var newName = pattern.BuildName(index, Path.GetExtension(source));

            // Every file stays in the folder it already lives in; only the name changes.
            var target = Path.Combine(Path.GetDirectoryName(source) ?? folder, newName);
            items.Add(new RenameItem(source, target));
        }

        var conflicts = new List<string>(pattern.Validate());
        conflicts.AddRange(DetectConflicts(items));

        return new RenamePlan(folder, items, conflicts);
    }

    /// <summary>
    /// Builds the plan that undoes a successfully executed plan by swapping every source and target.
    /// It is executed through the very same two-phase engine, so an undo is as safe as the rename.
    /// </summary>
    public RenamePlan BuildUndo(RenamePlan executedPlan)
    {
        ArgumentNullException.ThrowIfNull(executedPlan);

        var items = executedPlan.Items
            .Where(item => !item.IsNoOp)
            .Select(item => new RenameItem(item.TargetPath, item.SourcePath))
            .ToList();

        var conflicts = DetectConflicts(items).ToList();
        foreach (var item in items)
        {
            if (!File.Exists(item.SourcePath))
            {
                conflicts.Add($"{item.SourceFileName} (no longer at its renamed location)");
            }
        }

        return new RenamePlan(executedPlan.Folder, items, conflicts);
    }

    /// <summary>Finds duplicate target names and targets that would overwrite an unrelated file.</summary>
    private static IReadOnlyList<string> DetectConflicts(IReadOnlyList<RenameItem> items)
    {
        var conflicts = new List<string>();
        if (items.Count == 0)
        {
            return conflicts;
        }

        var sources = new HashSet<string>(
            items.Select(item => item.SourcePath),
            StringComparer.OrdinalIgnoreCase);

        var targets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!targets.Add(item.TargetPath))
            {
                conflicts.Add(item.TargetFileName);
            }
            else if (File.Exists(item.TargetPath) && !sources.Contains(item.TargetPath))
            {
                // The target is held by a file outside the batch: renaming would overwrite it.
                conflicts.Add(item.TargetFileName);
            }
        }

        return conflicts.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
