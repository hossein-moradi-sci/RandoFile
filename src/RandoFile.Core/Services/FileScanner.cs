using RandoFile.Core.Abstractions;

namespace RandoFile.Core.Services;

/// <summary>Enumerates the files of a folder. Any file type is accepted; only names are inspected.</summary>
public sealed class FileScanner : IFileScanner
{
    /// <summary>Hidden and system files are ignored so the batch never touches Windows bookkeeping files.</summary>
    public bool SkipHiddenAndSystem { get; init; } = true;

    /// <summary>Files whose name starts with this marker belong to an interrupted rename and are ignored.</summary>
    public string TempPrefix { get; init; } = RenameExecutor.TempFilePrefix;

    public IReadOnlyList<string> Scan(string folder, bool includeSubfolders = false)
    {
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            return Array.Empty<string>();
        }

        var option = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var results = new List<string>();

        foreach (var path in Directory.EnumerateFiles(folder, "*", option))
        {
            if (Path.GetFileName(path).StartsWith(TempPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            if (SkipHiddenAndSystem && IsHiddenOrSystem(path))
            {
                continue;
            }

            results.Add(path);
        }

        results.Sort(StringComparer.OrdinalIgnoreCase);
        return results;
    }

    private static bool IsHiddenOrSystem(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            return attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System);
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
