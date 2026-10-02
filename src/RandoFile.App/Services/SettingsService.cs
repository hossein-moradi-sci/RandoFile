using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RandoFile.Core;

namespace RandoFile.App.Services;

/// <summary>Loads and saves <see cref="AppSettings"/> under <c>%APPDATA%\RandoFile</c>.</summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };

    public SettingsService()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(root, AppInfo.FolderName);
        Directory.CreateDirectory(folder);
        FilePath = Path.Combine(folder, "settings.json");
    }

    /// <summary>Full path of the settings file, shown in the About window for support.</summary>
    public string FilePath { get; }

    /// <summary>Reads the settings; a missing or damaged file simply yields the defaults.</summary>
    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions) ?? new AppSettings();
        }
        catch (Exception)
        {
            // A corrupt settings file must never stop the application from starting.
            return new AppSettings();
        }
    }

    /// <summary>Writes the settings; failures are swallowed because they never block the work.</summary>
    public void Save(AppSettings settings)
    {
        try
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, SerializerOptions));
        }
        catch (Exception)
        {
            // Ignored on purpose.
        }
    }
}
