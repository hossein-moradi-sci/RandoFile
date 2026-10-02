using System;

namespace RandoFile.App.ViewModels;

/// <summary>One selectable entry of the settings window (a language or a theme).</summary>
public sealed class SettingsOption
{
    public SettingsOption(object value, string label, string? secondary = null, string? badge = null)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Label = label;
        Secondary = secondary;
        Badge = badge;
    }

    /// <summary>Either a <see cref="Localization.LanguageInfo"/> or a <see cref="Localization.ThemeMode"/>.</summary>
    public object Value { get; }

    /// <summary>Main text of the entry; languages are written in their own script.</summary>
    public string Label { get; }

    /// <summary>Secondary line, for example the English name, or <c>null</c>.</summary>
    public string? Secondary { get; }

    /// <summary>Short badge shown in front of the label, for example the country code.</summary>
    public string? Badge { get; }
}
