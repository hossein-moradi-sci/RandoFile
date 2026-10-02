using System;
using System.Globalization;
using System.Windows;
using RandoFile.App.Services;
using RandoFile.Core;

namespace RandoFile.App.Views;

/// <summary>Shows the version, the creator details and a manual update check.</summary>
public partial class AboutWindow : Window
{
    private readonly UpdateService _updateService = new();

    public AboutWindow()
    {
        InitializeComponent();

        LocalizationService.Instance.ApplyFlowDirection(this);
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;

        UpdateCopyrightText();
        Closed += OnClosed;
    }

    private static string L(string key) => LocalizationService.Instance.Get(key);

    private void UpdateCopyrightText() => CopyrightText.Text = LocalizationService.Instance.Format(
        "About.Copyright",
        DateTime.Now.Year.ToString(CultureInfo.InvariantCulture),
        AppInfo.Creator);

    private async void OnCheckUpdatesClick(object sender, RoutedEventArgs e)
    {
        CheckUpdatesButton.IsEnabled = false;
        UpdateStatusText.Text = L("Settings.Checking");

        try
        {
            var result = await _updateService.CheckAsync(App.Settings.RepositoryOwner, App.Settings.RepositoryName);

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

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        LocalizationService.Instance.ApplyFlowDirection(this);
        UpdateCopyrightText();
        UpdateStatusText.Text = string.Empty;
    }

    private void OnClosed(object? sender, EventArgs e) =>
        LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;
}
