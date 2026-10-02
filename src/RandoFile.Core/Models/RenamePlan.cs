namespace RandoFile.Core.Models;

/// <summary>
/// An immutable, already validated rename plan: the list of renames plus every conflict that
/// would stop the batch from running safely.
/// </summary>
public sealed class RenamePlan
{
    public RenamePlan(string folder, IReadOnlyList<RenameItem> items, IReadOnlyList<string>? conflicts = null)
    {
        Folder = folder ?? string.Empty;
        Items = items ?? Array.Empty<RenameItem>();
        Conflicts = conflicts ?? Array.Empty<string>();
    }

    /// <summary>Folder the plan was created for (informational; items carry their own directory).</summary>
    public string Folder { get; }

    public IReadOnlyList<RenameItem> Items { get; }

    /// <summary>Problems that make the plan unsafe to execute (duplicates and overwrites).</summary>
    public IReadOnlyList<string> Conflicts { get; }

    public bool HasConflicts => Conflicts.Count > 0;

    public bool IsEmpty => Items.Count == 0;

    /// <summary>Number of files that actually change name.</summary>
    public int ChangeCount => Items.Count(item => !item.IsNoOp);
}
