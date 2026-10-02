using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using RandoFile.App.Services;
using RandoFile.App.ViewModels;
using RandoFile.App.Views;

namespace RandoFile.App;

public partial class MainWindow : Window
{
    private const double PreviewColumnReserve = 26;

    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel(App.Settings, App.SettingsStore);
        DataContext = _viewModel;

        RestoreWindowSize();
        LocalizationService.Instance.ApplyFlowDirection(this);
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;

        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdatePreviewColumnWidths();
        await _viewModel.InitializeAsync();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;

        App.Settings.WindowWidth = Math.Max(MinWidth, Width);
        App.Settings.WindowHeight = Math.Max(MinHeight, Height);
        App.SettingsStore.Save(App.Settings);

        _viewModel.Dispose();
    }

    private void RestoreWindowSize()
    {
        if (App.Settings.WindowWidth > MinWidth)
        {
            Width = App.Settings.WindowWidth;
        }

        if (App.Settings.WindowHeight > MinHeight)
        {
            Height = App.Settings.WindowHeight;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e) =>
        LocalizationService.Instance.ApplyFlowDirection(this);

    /// <summary>Keeps the two preview columns the same width and always filling the panel.</summary>
    private void OnPreviewListSizeChanged(object sender, SizeChangedEventArgs e) => UpdatePreviewColumnWidths();

    private void UpdatePreviewColumnWidths()
    {
        var available = PreviewList.ActualWidth - PreviewColumnReserve;
        if (available <= 0 || OriginalColumn is null || NewColumn is null)
        {
            return;
        }

        var width = Math.Floor(available / 2);
        OriginalColumn.Width = width;
        NewColumn.Width = width;
    }

    private async void OnBrowseClick(object sender, RoutedEventArgs e) => await _viewModel.BrowseAsync();

    private async void OnPreviewClick(object sender, RoutedEventArgs e) => await _viewModel.PreviewAsync();

    private async void OnRandomizeClick(object sender, RoutedEventArgs e) => await _viewModel.RandomizeAsync();

    private async void OnUndoClick(object sender, RoutedEventArgs e) => await _viewModel.UndoAsync();

    private void OnCancelClick(object sender, RoutedEventArgs e) => _viewModel.Cancel();

    private void OnOpenFolderClick(object sender, RoutedEventArgs e) => _viewModel.OpenFolder();

    private void OnOpenReleasesClick(object sender, RoutedEventArgs e) => _viewModel.OpenReleasePage();

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        _viewModel.PersistNamingSettings();
        new SettingsWindow { Owner = this }.ShowDialog();
    }

    private void OnAboutClick(object sender, RoutedEventArgs e) => new AboutWindow { Owner = this }.ShowDialog();

    private void OnHelpClick(object sender, RoutedEventArgs e) => HelpWindow.ShowHelp(this);

    /// <summary>
    /// F1 is handled in the preview stage, which runs before the focused text box, so the guide
    /// also opens while the user is typing a custom prefix.
    /// </summary>
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F1)
        {
            return;
        }

        e.Handled = true;
        HelpWindow.ShowHelp(this);
    }
}
