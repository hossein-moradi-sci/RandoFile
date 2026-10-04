using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RandoFile.App.Services;
using RandoFile.Core;
using RandoFile.App.Views;

namespace RandoFile.App;

public partial class App : Application
{
    /// <summary>Loads and saves <c>%APPDATA%\RandoFile\settings.json</c>.</summary>
    public static SettingsService SettingsStore { get; } = new();

    /// <summary>Settings of the current session; written back whenever the user changes something.</summary>
    public static AppSettings Settings { get; private set; } = new();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

        SplashWindow? splash = null;

        // Anything below can throw before a window exists, so the whole startup is guarded and the
        // full exception is written to the crash log; the dialog only shows it when resources work.
        try
        {
            // The splash is its own dark surface, so it is shown before the theme is applied.
            splash = new SplashWindow();
            splash.Show();

            Settings = SettingsStore.Load();

            // Windows can launch the app with a path, which is how a file association, a
            // shortcut or "Open with" reaches it. The folder that path names wins over the one
            // remembered from last time, because it is the one the user just asked for.
            var requestedFolder = CommandLineTarget.ResolveFolderFromArguments(e.Args);

            if (requestedFolder is not null)
            {
                Settings.LastFolder = requestedFolder;
            }

            // Theme first, then language, so the very first frame of the main window is correct.
            ThemeService.Apply(ThemeService.Parse(Settings.Theme));
            LocalizationService.Instance.Initialize(Settings.Language);

            var window = new MainWindow();
            MainWindow = window;

            await splash.CloseAsync().ConfigureAwait(true);
            splash = null;

            window.Show();
        }
        catch (Exception ex)
        {
            CrashLog.Write("OnStartup", ex);
            splash?.Close();
            DialogService.Error(ex.Message);
            Shutdown(1);
        }
    }

    /// <summary>Last line of defence: report the problem instead of closing silently.</summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        CrashLog.Write("Dispatcher", e.Exception);
        DialogService.Error(e.Exception.Message);
        e.Handled = true;
    }

    private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        CrashLog.Write("AppDomain", e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString()));
    }
}
