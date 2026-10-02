using System;
using System.Linq;
using System.Windows;
using RandoFile.App.Localization;
using Microsoft.Win32;

namespace RandoFile.App.Services;

/// <summary>Swaps the light and dark brush dictionaries at runtime.</summary>
public static class ThemeService
{
    private const string ThemeFolderMarker = "/Themes/";

    /// <summary>Colour scheme selected by the user.</summary>
    public static ThemeMode Mode { get; private set; } = ThemeMode.System;

    /// <summary>True when the dark brushes are currently loaded.</summary>
    public static bool IsDark { get; private set; }

    public static event EventHandler? Changed;

    public static void Apply(ThemeMode mode)
    {
        Mode = mode;
        IsDark = ResolveIsDark(mode);

        var application = Application.Current;
        if (application is not null)
        {
            var dictionaries = application.Resources.MergedDictionaries;
            var file = IsDark ? "Dark.xaml" : "Light.xaml";

            var replacement = new ResourceDictionary
            {
                Source = new Uri($"Resources/Themes/{file}", UriKind.Relative),
            };

            var existing = dictionaries.FirstOrDefault(IsThemeDictionary);
            if (existing is null)
            {
                dictionaries.Insert(0, replacement);
            }
            else
            {
                dictionaries[dictionaries.IndexOf(existing)] = replacement;
            }
        }

        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static ThemeMode Parse(string? value) =>
        Enum.TryParse<ThemeMode>(value, ignoreCase: true, out var mode) ? mode : ThemeMode.System;

    private static bool IsThemeDictionary(ResourceDictionary dictionary)
    {
        var source = dictionary.Source?.OriginalString;
        return source is not null
               && source.Contains(ThemeFolderMarker, StringComparison.OrdinalIgnoreCase)
               && (source.EndsWith("Light.xaml", StringComparison.OrdinalIgnoreCase)
                   || source.EndsWith("Dark.xaml", StringComparison.OrdinalIgnoreCase));
    }

    private static bool ResolveIsDark(ThemeMode mode) => mode switch
    {
        ThemeMode.Dark => true,
        ThemeMode.Light => false,
        _ => IsSystemDark(),
    };

    /// <summary>Reads the Windows "app mode" preference; defaults to light when it is unavailable.</summary>
    private static bool IsSystemDark()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            return key?.GetValue("AppsUseLightTheme") is int light && light == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
