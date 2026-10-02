using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using RandoFile.App.Localization;

namespace RandoFile.App.Services;

/// <summary>
/// Swaps the <c>Strings.&lt;code&gt;.xaml</c> resource dictionary at runtime, which makes every
/// <c>DynamicResource</c> in the UI update immediately, and flips the layout to right-to-left
/// for Persian and Arabic.
/// </summary>
public sealed class LocalizationService
{
    private const string StringsFolderMarker = "/Strings/";

    private static readonly Lazy<LocalizationService> LazyInstance = new(() => new LocalizationService());

    private LocalizationService()
    {
    }

    public static LocalizationService Instance => LazyInstance.Value;

    public LanguageInfo Current { get; private set; } = LanguageInfo.English;

    public bool IsRightToLeft => Current.IsRightToLeft;

    /// <summary>Raised after the language was applied, so views can refresh their own text.</summary>
    public event EventHandler? LanguageChanged;

    /// <summary>Applies the language saved in the settings without raising change notifications.</summary>
    public void Initialize(string? code) => Apply(LanguageInfo.FromCode(code));

    /// <summary>Switches the language at runtime and updates every open window.</summary>
    public void SetLanguage(string? code)
    {
        var language = LanguageInfo.FromCode(code);
        if (string.Equals(language.Code, Current.Code, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Apply(language);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Looks a translated string up by key; the key itself is returned when it is missing.</summary>
    public string Get(string key)
    {
        var value = Application.Current?.TryFindResource(key);
        return value as string ?? key;
    }

    /// <summary>Looks a translated string up and fills in its placeholders.</summary>
    public string Format(string key, params object?[] arguments)
    {
        var template = Get(key);

        try
        {
            return string.Format(CultureInfo.InvariantCulture, template, arguments);
        }
        catch (FormatException)
        {
            return template;
        }
    }

    /// <summary>Left-to-right or right-to-left, matching the current language.</summary>
    public FlowDirection FlowDirection => IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    /// <summary>Applies the direction of the current language to an element that was just created.</summary>
    public void ApplyFlowDirection(DependencyObject target) => target.SetValue(FrameworkElement.FlowDirectionProperty, FlowDirection);

    private void Apply(LanguageInfo language)
    {
        SwapDictionary(language);
        Current = language;

        var culture = CultureInfo.GetCultureInfo(language.Code);
        CultureInfo.CurrentUICulture = culture;

        ApplyFlowDirectionToOpenWindows();
    }

    private void ApplyFlowDirectionToOpenWindows()
    {
        if (Application.Current is null)
        {
            return;
        }

        foreach (Window window in Application.Current.Windows)
        {
            window.FlowDirection = FlowDirection;
        }
    }

    private static void SwapDictionary(LanguageInfo language)
    {
        var application = Application.Current;
        if (application is null)
        {
            return;
        }

        var dictionaries = application.Resources.MergedDictionaries;
        var source = new Uri($"Resources/Strings/Strings.{language.Code}.xaml", UriKind.Relative);
        var replacement = new ResourceDictionary { Source = source };

        var existing = dictionaries.FirstOrDefault(IsStringsDictionary);
        if (existing is null)
        {
            dictionaries.Add(replacement);
            return;
        }

        dictionaries[dictionaries.IndexOf(existing)] = replacement;
    }

    private static bool IsStringsDictionary(ResourceDictionary dictionary) =>
        dictionary.Source?.OriginalString.Contains(StringsFolderMarker, StringComparison.OrdinalIgnoreCase) == true;
}
