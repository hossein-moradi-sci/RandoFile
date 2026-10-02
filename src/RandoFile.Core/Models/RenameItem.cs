namespace RandoFile.Core.Models;

/// <summary>A single planned rename: one existing file and the name it should end up with.</summary>
public sealed class RenameItem
{
    public RenameItem(string sourcePath, string targetPath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new ArgumentException("A source path is required.", nameof(sourcePath));
        }

        if (string.IsNullOrWhiteSpace(targetPath))
        {
            throw new ArgumentException("A target path is required.", nameof(targetPath));
        }

        SourcePath = sourcePath;
        TargetPath = targetPath;
    }

    /// <summary>Full path of the file as it exists today.</summary>
    public string SourcePath { get; }

    /// <summary>Full path the file should have after the rename.</summary>
    public string TargetPath { get; }

    public string SourceFileName => Path.GetFileName(SourcePath);

    public string TargetFileName => Path.GetFileName(TargetPath);

    /// <summary>Extension of the original file, always preserved by the planner.</summary>
    public string Extension => Path.GetExtension(SourcePath);

    /// <summary>Folder that contains the file (the rename never moves a file across folders).</summary>
    public string Directory => Path.GetDirectoryName(SourcePath) ?? string.Empty;

    /// <summary>True when the shuffle happened to keep the current name, so nothing has to be written.</summary>
    public bool IsNoOp => string.Equals(SourcePath, TargetPath, StringComparison.OrdinalIgnoreCase);

    public override string ToString() => $"{SourceFileName} -> {TargetFileName}";
}
