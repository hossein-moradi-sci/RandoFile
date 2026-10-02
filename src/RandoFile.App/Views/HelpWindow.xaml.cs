using System;
using System.Linq;
using System.Windows;
using RandoFile.App.Services;

namespace RandoFile.App.Views;

/// <summary>
/// Shows the in-app guide. It is opened modeless on purpose: the user can follow a step in the
/// text while the main window stays usable, which is the whole point of a "getting started" page.
/// </summary>
public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();

        LocalizationService.Instance.ApplyFlowDirection(this);
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;

        Closed += OnClosed;
    }

    /// <summary>
    /// Opens the guide, or brings the copy that is already open to the front. F1 can be pressed
    /// over and over and must never stack a second window on top of the first.
    /// </summary>
    public static void ShowHelp(Window? owner)
    {
        if (Application.Current?.Windows.OfType<HelpWindow>().FirstOrDefault() is { } open)
        {
            if (open.WindowState == WindowState.Minimized)
            {
                open.WindowState = WindowState.Normal;
            }

            open.Activate();
            return;
        }

        new HelpWindow { Owner = owner }.Show();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    private void OnLanguageChanged(object? sender, EventArgs e) =>
        LocalizationService.Instance.ApplyFlowDirection(this);

    private void OnClosed(object? sender, EventArgs e) =>
        LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;
}
