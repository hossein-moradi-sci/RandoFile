using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RandoFile.App.Localization;
using RandoFile.App.Services;
using RandoFile.Core;
using RandoFile.App.ViewModels;

namespace RandoFile.App.Views;

/// <summary>
/// Settings dialog. Language and theme are applied the moment they are picked so the user sees the
/// result immediately; Cancel puts everything back the way it was.
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly AppSettings _original;
    private readonly AppSettings _working;
    private readonly UpdateService _updateService = new();
    private bool _ready;

    public SettingsWindow()
    {
        InitializeComponent();

        _original = App.Settings.Clone();
        _working = App.Settings.Clone();

        LocalizationService.Instance.ApplyFlowDirection(this);

        BuildLanguageOptions();
        BuildThemeOptions();

        IncludeSubfoldersCheck.IsChecked = _working.IncludeSubfolders;
        CheckUpdatesCheck.IsChecked = _working.CheckForUpdates;
        UpdateVersionText();

        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
        Closed += OnClosed;

        _ready = true;
    }

    private static string L(string key) => LocalizationService.Instance.Get(key);

    private void BuildLanguageOptions()
    {
        var options = LanguageInfo.All
            .Select(language => new SettingsOption(language, language.NativeName, language.EnglishName, language.RegionCode))
            .ToList();

        var current = LanguageInfo.FromCode(_working.Language);
        LanguageList.ItemsSource = options;
        LanguageList.SelectedItem = options.First(option => ReferenceEquals(option.Value, current));
    }

    private void BuildThemeOptions()
    {
        var options = new[]
        {
            new SettingsOption(ThemeMode.Light, L("Settings.ThemeLight")),
            new SettingsOption(ThemeMode.Dark, L("Settings.ThemeDark")),
            new SettingsOption(ThemeMode.System, L("Settings.ThemeSystem")),
        };

        var current = ThemeService.Parse(_working.Theme);
        ThemeList.ItemsSource = options;
        ThemeList.SelectedItem = options.First(option => (ThemeMode)option.Value == current);
    }

    private void UpdateVersionText() => VersionText.Text = LocalizationService.Instance.Format(
        "Settings.CurrentVersion",
        AppInfo.DisplayVersion);

    private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_ready || LanguageList.SelectedItem is not SettingsOption { Value: LanguageInfo language })
        {
            return;
        }

        _working.Language = language.Code;
        LocalizationService.Instance.SetLanguage(language.Code);
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_ready || ThemeList.SelectedItem is not SettingsOption { Value: ThemeMode mode })
        {
            return;
        }

        _working.Theme = mode.ToString();
        ThemeService.Apply(mode);
    }

    private void OnOptionChanged(object sender, RoutedEventArgs e)
    {
        if (!_ready)
        {
            return;
        }

        _working.IncludeSubfolders = IncludeSubfoldersCheck.IsChecked == true;
        _working.CheckForUpdates = CheckUpdatesCheck.IsChecked == true;
        UpdateStatusText.Text = string.Empty;
    }

    private async void OnCheckUpdatesClick(object sender, RoutedEventArgs e)
    {
        CheckUpdatesButton.IsEnabled = false;
        UpdateStatusText.Text = L("Settings.Checking");

        try
        {
            var result = await _updateService.CheckAsync(_working.RepositoryOwner, _working.RepositoryName);

            UpdateStatusText.Text = result.Status switch
            {
                UpdateStatus.Available => LocalizationService.Instance.Format("Settings.NewVersion", result.Version ?? string.Empty),
                UpdateStatus.UpToDate or UpdateStatus.NoRelease => L("Settings.UpToDate"),
                _ => L("Settings.UpdateFailed"),
            };
        }
        finally
        {
            CheckUpdatesButton.IsEnabled = true;
        }
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        App.Settings.Language = _working.Language;
        App.Settings.Theme = _working.Theme;
        App.Settings.IncludeSubfolders = _working.IncludeSubfolders;
        App.Settings.CheckForUpdates = _working.CheckForUpdates;
        App.SettingsStore.Save(App.Settings);

        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void OnHelpClick(object sender, RoutedEventArgs e) => HelpWindow.ShowHelp(this);

    /// <summary>Same F1 contract as the main window, so the guide is reachable from the dialog too.</summary>
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F1)
        {
            return;
        }

        e.Handled = true;
        HelpWindow.ShowHelp(this);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        // The language list keeps its own labels (each language is written in its own script),
        // only the theme names and the version line have to be rebuilt.
        _ready = false;
        BuildThemeOptions();
        UpdateVersionText();
        _ready = true;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;

        if (DialogResult == true)
        {
            return;
        }

        // Cancelled, or closed with the X button: undo the live language and theme preview.
        LocalizationService.Instance.SetLanguage(_original.Language);
        ThemeService.Apply(ThemeService.Parse(_original.Theme));
    }
}
