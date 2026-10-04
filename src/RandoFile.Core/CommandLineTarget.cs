using System;
using System.IO;

namespace RandoFile.Core;

/// <summary>
/// Works out what folder the user meant when Windows hands RandoFile a path, which is how the
/// program is opened when it is launched from a file association, a shortcut or "Open with".
/// </summary>
/// <remarks>
/// RandoFile randomizes the contents of a folder, so a folder argument is used as it stands and
/// a file argument means "the folder that file is in". Anything that does not resolve to a folder
/// that exists is ignored rather than reported: a shell verb can be invoked long after the path it
/// was given has been moved or deleted, and a dialog about that would be worse than doing nothing.
/// </remarks>
public static class CommandLineTarget
{
    /// <summary>The folder the app should open, or <c>null</c> to keep whatever it had.</summary>
    public static string? ResolveFolder(string? argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            return null;
        }

        // Explorer's "Open with" and shortcut launches can arrive quoted even though the shell
        // usually strips them, and quoting is not something a caller should have to think about.
        var path = argument.Trim().Trim('"').Trim();

        if (path.Length == 0)
        {
            return null;
        }

        try
        {
            if (Directory.Exists(path))
            {
                return Path.GetFullPath(path);
            }

            if (File.Exists(path))
            {
                var directory = Path.GetDirectoryName(Path.GetFullPath(path));

                return string.IsNullOrEmpty(directory) ? null : directory;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException or IOException or UnauthorizedAccessException)
        {
            // A path that cannot even be inspected is treated the same as one that does not exist.
        }

        return null;
    }

    /// <summary>Picks the first usable folder out of a command line, ignoring switches.</summary>
    public static string? ResolveFolderFromArguments(string[]? arguments)
    {
        if (arguments is null)
        {
            return null;
        }

        foreach (var argument in arguments)
        {
            if (string.IsNullOrWhiteSpace(argument) || argument.StartsWith("-", StringComparison.Ordinal))
            {
                continue;
            }

            var folder = ResolveFolder(argument);

            if (folder is not null)
            {
                return folder;
            }
        }

        return null;
    }
}