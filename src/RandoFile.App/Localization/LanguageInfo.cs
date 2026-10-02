using System.Collections.Generic;
using System.Linq;

namespace RandoFile.App.Localization;

/// <summary>Describes one of the languages the interface can be shown in.</summary>
/// <param name="Code">Culture code, also the suffix of the resource dictionary file.</param>
/// <param name="NativeName">Name of the language written in the language itself.</param>
/// <param name="EnglishName">Name used in logs and diagnostics.</param>
/// <param name="RegionCode">Two letter code drawn as a badge next to the language.</param>
/// <param name="IsRightToLeft">True for Persian and Arabic.</param>
public sealed record LanguageInfo(
    string Code,
    string NativeName,
    string EnglishName,
    string RegionCode,
    bool IsRightToLeft)
{
    public static LanguageInfo English { get; } = new("en", "English", "English", "GB", false);

    public static LanguageInfo Persian { get; } = new("fa", "فارسی", "Persian", "IR", true);

    public static LanguageInfo French { get; } = new("fr", "Français", "French", "FR", false);

    public static LanguageInfo Arabic { get; } = new("ar", "العربية", "Arabic", "SA", true);

    /// <summary>Every supported language, in the order shown in the settings window.</summary>
    public static IReadOnlyList<LanguageInfo> All { get; } = [Persian, English, French, Arabic];

    /// <summary>Resolves a culture code, falling back to English for anything unknown.</summary>
    public static LanguageInfo FromCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return English;
        }

        return All.FirstOrDefault(language => string.Equals(language.Code, code, System.StringComparison.OrdinalIgnoreCase))
               ?? English;
    }
}

/// <summary>Which colour scheme the user picked.</summary>
public enum ThemeMode
{
    /// <summary>Always light.</summary>
    Light = 0,

    /// <summary>Always dark.</summary>
    Dark = 1,

    /// <summary>Follow the "app mode" setting of Windows.</summary>
    System = 2,
}
