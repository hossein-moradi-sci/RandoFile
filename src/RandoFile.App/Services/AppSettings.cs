using RandoFile.Core;

namespace RandoFile.App.Services;

/// <summary>
/// Everything the application remembers between runs. Stored as JSON in the user's
/// roaming profile, never next to the executable.
/// </summary>
public sealed class AppSettings
{
    public string Language { get; set; } = "en";

    public string Theme { get; set; } = "System";

    public string LastFolder { get; set; } = string.Empty;

    public string PatternPreset { get; set; } = "Image";

    public string CustomPrefix { get; set; } = string.Empty;

    public int StartNumber { get; set; } = 1;

    public int Digits { get; set; }

    public bool IncludeSubfolders { get; set; }

    public bool CheckForUpdates { get; set; } = true;

    public string RepositoryOwner { get; set; } = AppInfo.RepositoryOwner;

    public string RepositoryName { get; set; } = AppInfo.RepositoryName;

    public double WindowWidth { get; set; } = 1140;

    public double WindowHeight { get; set; } = 720;

    /// <summary>Detached copy used to preview changes in the settings window before saving.</summary>
    public AppSettings Clone() => (AppSettings)MemberwiseClone();
}
