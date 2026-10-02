using System;
using System.IO;

namespace RandoFile.Core;

/// <summary>
/// Single source of truth for the application identity, shared by the UI, the about window,
/// the update service and the executable metadata.
/// </summary>
public static class AppInfo
{
    /// <summary>
    /// Brand shown in the interface: the logo, the splash screen, the window title bar and the
    /// executable file name. A brand is never translated, in any language.
    /// </summary>
    public const string Brand = "RandoFile";

    /// <summary>
    /// Full product name used in the Windows file properties, the about window and the release
    /// assets. The <c>HM</c> prefix keeps the product traceable to its author.
    /// </summary>
    public const string Name = "HM File Randomizer";

    /// <summary>One line describing what the program does; shown under the logo.</summary>
    public const string Tagline = "Randomize any file in any folder";

    public const string Version = "1.0.0";

    public const string VersionPrefix = "v";

    public const string Creator = "Hossein Moradi";

    public const string CreatorEmail = "hossein.moradi.sci@gmail.com";

    /// <summary>Per-user folder holding the settings and the crash log.</summary>
    public const string FolderName = "RandoFile";

    /// <summary>Directory used for diagnostics, e.g. <c>%LOCALAPPDATA%\RandoFile</c>.</summary>
    public static string LogFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FolderName);

    /// <summary>GitHub repository the update service watches for new releases.</summary>
    public const string RepositoryOwner = "hossein-moradi-sci";

    public const string RepositoryName = "hm-file-randomizer";

    public static string RepositoryUrl => $"https://github.com/{RepositoryOwner}/{RepositoryName}";

    public static string ReleasesUrl => $"{RepositoryUrl}/releases/latest";

    public static string DisplayVersion => VersionPrefix + Version;
}
