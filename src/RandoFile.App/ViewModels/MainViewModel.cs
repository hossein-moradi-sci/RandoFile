using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RandoFile.App.Services;
using RandoFile.Core.Abstractions;
using RandoFile.Core.Models;
using RandoFile.Core;
using RandoFile.Core.Services;
using Microsoft.Win32;

namespace RandoFile.App.ViewModels;

/// <summary>
/// Drives the main window: scanning the folder, planning a safe rename, showing the preview,
/// executing it with progress and undoing the last run.
/// </summary>
public sealed class MainViewModel : ObservableObject, IDisposable
{
    /// <summary>A preview of an enormous folder stays responsive by showing only the first rows.</summary>
    private const int MaxPreviewRows = 2000;

    private readonly IFileScanner _scanner;
    private readonly RenamePlanBuilder _planBuilder;
    private readonly RenameExecutor _executor;
    private readonly UpdateService _updateService = new();
    private readonly AppSettings _settings;
    private readonly SettingsService _settingsService;

    private IReadOnlyList<string> _files = Array.Empty<string>();
    private RenamePlan? _plan;
    private string _planKey = string.Empty;
    private RenamePlan? _lastExecutedPlan;
    private CancellationTokenSource? _cancellation;

    private string _folderPath = string.Empty;
    private NamingPreset _preset = NamingPreset.Image;
    private string _customPrefix = string.Empty;
    private int _startNumber = 1;
    private int _digits;
    private bool _isBusy;
    private double _progressValue;
    private string _statusText;
    private StatusLevel _statusLevel = StatusLevel.Info;
    private IReadOnlyList<PreviewRow> _rows = Array.Empty<PreviewRow>();
    private bool _isPreviewTruncated;
    private bool _isUpdateAvailable;
    private string _updateText = string.Empty;
    private string? _latestReleaseUrl;
    private bool _disposed;

    public MainViewModel(AppSettings settings, SettingsService settingsService)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

        _scanner = new FileScanner();
        _planBuilder = new RenamePlanBuilder();
        _executor = new RenameExecutor();

        _statusText = LocalizationService.Instance.Get("Status.Ready");
        _folderPath = settings.LastFolder;
        _customPrefix = settings.CustomPrefix;
        _startNumber = settings.StartNumber < 0 ? 1 : settings.StartNumber;
        _digits = settings.Digits is < 0 or > 12 ? 0 : settings.Digits;
        _preset = ParsePreset(settings.PatternPreset);

        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
    }

    // ---------------------------------------------------------------- folder

    public string FolderPath
    {
        get => _folderPath;
        private set
        {
            if (SetProperty(ref _folderPath, value))
            {
                OnPropertiesChanged(nameof(FolderDisplay), nameof(HasFolder), nameof(CanPreview));
            }
        }
    }

    public bool HasFolder => !string.IsNullOrWhiteSpace(FolderPath) && Directory.Exists(FolderPath);

    /// <summary>Path shown in the folder box, or the localized placeholder when nothing is selected.</summary>
    public string FolderDisplay => string.IsNullOrWhiteSpace(FolderPath)
        ? LocalizationService.Instance.Get("Folder.Placeholder")
        : FolderPath;

    public int FileCount { get; private set; }

    public string FileCountText => LocalizationService.Instance.Format(
        "Folder.FilesFound",
        FileCount.ToString(CultureInfo.InvariantCulture));

    // ---------------------------------------------------------------- naming

    public NamingPreset Preset
    {
        get => _preset;
        private set
        {
            if (!SetProperty(ref _preset, value))
            {
                return;
            }

            OnPropertiesChanged(
                nameof(IsImagePreset),
                nameof(IsFilePreset),
                nameof(IsCustomPreset),
                nameof(IsCustomPrefixEnabled),
                nameof(ExampleText),
                nameof(CanRandomize));
        }
    }

    public bool IsImagePreset
    {
        get => Preset == NamingPreset.Image;
        set { if (value) Preset = NamingPreset.Image; }
    }

    public bool IsFilePreset
    {
        get => Preset == NamingPreset.File;
        set { if (value) Preset = NamingPreset.File; }
    }

    public bool IsCustomPreset
    {
        get => Preset == NamingPreset.Custom;
        set { if (value) Preset = NamingPreset.Custom; }
    }

    public bool IsCustomPrefixEnabled => Preset == NamingPreset.Custom;

    public string CustomPrefix
    {
        get => _customPrefix;
        set
        {
            if (SetProperty(ref _customPrefix, value))
            {
                OnPropertiesChanged(nameof(ExampleText), nameof(CanRandomize));
            }
        }
    }

    public int StartNumber
    {
        get => _startNumber;
        set
        {
            if (SetProperty(ref _startNumber, Math.Max(0, value)))
            {
                OnPropertiesChanged(nameof(ExampleText), nameof(CanRandomize));
            }
        }
    }

    public int Digits
    {
        get => _digits;
        set
        {
            if (SetProperty(ref _digits, Math.Clamp(value, 0, 12)))
            {
                OnPropertiesChanged(nameof(ExampleText), nameof(CanRandomize));
            }
        }
    }

    /// <summary>Live example of the first file name the current settings would produce.</summary>
    public string ExampleText => LocalizationService.Instance.Format(
        "Naming.Example",
        BuildPattern().BuildName(0, ".jpg"));

    // ---------------------------------------------------------------- preview

    public IReadOnlyList<PreviewRow> Rows
    {
        get => _rows;
        private set
        {
            if (SetProperty(ref _rows, value))
            {
                OnPropertiesChanged(nameof(HasPreview), nameof(PreviewSummaryText), nameof(CanRandomize), nameof(CanPreview));
            }
        }
    }

    public bool HasPreview => Rows.Count > 0;

    public bool IsPreviewTruncated
    {
        get => _isPreviewTruncated;
        private set => SetProperty(ref _isPreviewTruncated, value);
    }

    /// <summary>Explains that a very large folder only shows its first rows.</summary>
    public string TruncatedText => LocalizationService.Instance.Format(
        "Preview.Truncated",
        MaxPreviewRows.ToString(CultureInfo.InvariantCulture));

    public string PreviewSummaryText
    {
        get
        {
            if (_plan is null || _plan.Items.Count == 0)
            {
                return LocalizationService.Instance.Get("Preview.Empty");
            }

            var localization = LocalizationService.Instance;
            var total = localization.Format("Preview.Total", _plan.Items.Count.ToString(CultureInfo.InvariantCulture));
            var changes = localization.Format("Preview.Changes", _plan.ChangeCount.ToString(CultureInfo.InvariantCulture));

            if (_plan.HasConflicts)
            {
                var conflicts = localization.Format("Preview.Conflicts", _plan.Conflicts.Count.ToString(CultureInfo.InvariantCulture));
                return $"{total}  ·  {changes}  ·  {conflicts}";
            }

            return $"{total}  ·  {changes}";
        }
    }

    // ---------------------------------------------------------------- progress / status

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertiesChanged(nameof(IsIdle), nameof(CanPreview), nameof(CanRandomize), nameof(CanUndo), nameof(CanCancel));
            }
        }
    }

    public bool IsIdle => !IsBusy;

    public bool CanCancel => IsBusy;

    public double ProgressValue
    {
        get => _progressValue;
        private set => SetProperty(ref _progressValue, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public StatusLevel StatusLevel
    {
        get => _statusLevel;
        private set => SetProperty(ref _statusLevel, value);
    }

    public bool CanPreview => !IsBusy && HasFolder;

    public bool CanRandomize => !IsBusy && HasPreview && _plan is { HasConflicts: false, IsEmpty: false };

    public bool CanUndo => !IsBusy && _lastExecutedPlan is not null;

    // ---------------------------------------------------------------- updates

    public bool IsUpdateAvailable
    {
        get => _isUpdateAvailable;
        private set => SetProperty(ref _isUpdateAvailable, value);
    }

    public string UpdateText
    {
        get => _updateText;
        private set => SetProperty(ref _updateText, value);
    }

    // ---------------------------------------------------------------- commands

    /// <summary>Restores the last folder and scans it, then optionally checks GitHub for updates.</summary>
    public async Task InitializeAsync()
    {
        if (HasFolder)
        {
            await RefreshFolderAsync();
        }
        else
        {
            StatusText = LocalizationService.Instance.Get("Status.Ready");
        }

        if (_settings.CheckForUpdates)
        {
            await CheckForUpdatesAsync(silent: true);
        }
    }

    public async Task BrowseAsync()
    {
        var dialog = new OpenFolderDialog
        {
            Title = LocalizationService.Instance.Get("Folder.Placeholder"),
            Multiselect = false,
        };

        if (HasFolder)
        {
            dialog.InitialDirectory = FolderPath;
        }

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        FolderPath = dialog.FolderName;
        _settings.LastFolder = FolderPath;
        _settingsService.Save(_settings);

        await RefreshFolderAsync();
    }

    /// <summary>Rescans the folder and clears any stale preview.</summary>
    public async Task RefreshFolderAsync()
    {
        ClearPreview();
        _lastExecutedPlan = null;
        OnPropertyChanged(nameof(CanUndo));

        if (!HasFolder)
        {
            FileCount = 0;
            OnPropertiesChanged(nameof(FileCount), nameof(FileCountText));
            return;
        }

        IsBusy = true;
        StatusText = LocalizationService.Instance.Get("Status.Scanning");
        StatusLevel = StatusLevel.Info;

        try
        {
            var folder = FolderPath;
            var recurse = _settings.IncludeSubfolders;
            _files = await Task.Run(() => _scanner.Scan(folder, recurse)).ConfigureAwait(true);

            FileCount = _files.Count;
            OnPropertiesChanged(nameof(FileCount), nameof(FileCountText));
            StatusText = LocalizationService.Instance.Format(
                "Folder.FilesFound",
                FileCount.ToString(CultureInfo.InvariantCulture));
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Builds a fresh shuffled plan and shows it, so the user sees the exact upcoming rename.</summary>
    public async Task PreviewAsync()
    {
        if (!HasFolder)
        {
            DialogService.Warn(LocalizationService.Instance.Get("Msg.NoFolder"));
            return;
        }

        IsBusy = true;
        StatusText = LocalizationService.Instance.Get("Status.BuildingPreview");
        StatusLevel = StatusLevel.Info;

        try
        {
            var folder = FolderPath;
            var recurse = _settings.IncludeSubfolders;
            var pattern = BuildPattern();

            var files = await Task.Run(() => _scanner.Scan(folder, recurse)).ConfigureAwait(true);
            _files = files;
            FileCount = files.Count;
            OnPropertiesChanged(nameof(FileCount), nameof(FileCountText));

            if (files.Count == 0)
            {
                ClearPreview();
                StatusText = LocalizationService.Instance.Get("Msg.NoFiles");
                StatusLevel = StatusLevel.Warning;
                DialogService.Warn(LocalizationService.Instance.Get("Msg.NoFiles"));
                return;
            }

            var plan = await Task.Run(() => _planBuilder.Build(files, pattern, shuffle: true)).ConfigureAwait(true);
            ApplyPlan(plan, pattern);
        }
        catch (Exception exception)
        {
            ClearPreview();
            StatusText = exception.Message;
            StatusLevel = StatusLevel.Error;
            DialogService.Error(exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Shuffles if needed, confirms, then renames. The plan that runs is exactly the plan on screen.
    /// </summary>
    public async Task RandomizeAsync()
    {
        if (!HasFolder)
        {
            DialogService.Warn(LocalizationService.Instance.Get("Msg.NoFolder"));
            return;
        }

        var pattern = BuildPattern();

        // Pressing Randomize right after changing something rebuilds the plan first.
        if (_plan is null || _planKey != BuildPlanKey(pattern))
        {
            await PreviewAsync();
        }

        if (_plan is null || _plan.IsEmpty)
        {
            return;
        }

        var localization = LocalizationService.Instance;

        if (_plan.HasConflicts)
        {
            DialogService.Error(localization.Format(
                "Msg.ConflictsBody",
                string.Join(Environment.NewLine, _plan.Conflicts)));

            StatusText = localization.Format("Status.Conflicts", _plan.Conflicts.Count.ToString(CultureInfo.InvariantCulture));
            StatusLevel = StatusLevel.Error;
            return;
        }

        if (_plan.ChangeCount == 0)
        {
            DialogService.Info(localization.Get("Status.NothingToDo"));
            return;
        }

        var count = _plan.ChangeCount;
        if (!DialogService.Confirm(
                localization.Format("Msg.ConfirmBody", count.ToString(CultureInfo.InvariantCulture), FolderPath),
                localization.Get("Msg.ConfirmTitle")))
        {
            return;
        }

        var plan = _plan;
        var result = await ExecuteAsync(plan, localization.Format("Status.Renaming", count.ToString(CultureInfo.InvariantCulture)));

        if (result.Success)
        {
            _lastExecutedPlan = plan;
            var seconds = FormatSeconds(result.Duration);
            StatusText = localization.Format("Status.Completed", result.Completed.ToString(CultureInfo.InvariantCulture), seconds);
            StatusLevel = StatusLevel.Success;
            DialogService.Success(localization.Format("Msg.RenameDone", result.Completed.ToString(CultureInfo.InvariantCulture), seconds));
        }
        else
        {
            _lastExecutedPlan = null;
            ReportFailure(result);
        }

        OnPropertyChanged(nameof(CanUndo));

        // The folder now holds new names: rescans and drops the stale plan.
        await RefreshFolderAsync();
    }

    /// <summary>Restores the original names of the last successful run.</summary>
    public async Task UndoAsync()
    {
        var localization = LocalizationService.Instance;

        if (_lastExecutedPlan is null)
        {
            DialogService.Info(localization.Get("Msg.UndoNothing"));
            return;
        }

        var executed = _lastExecutedPlan;
        var count = executed.ChangeCount;

        if (!DialogService.Confirm(
                localization.Format("Msg.UndoBody", count.ToString(CultureInfo.InvariantCulture)),
                localization.Get("Msg.UndoTitle")))
        {
            return;
        }

        var undoPlan = _planBuilder.BuildUndo(executed);
        if (undoPlan.HasConflicts)
        {
            DialogService.Error(localization.Format("Msg.UndoFailed", string.Join(Environment.NewLine, undoPlan.Conflicts)));
            return;
        }

        var result = await ExecuteAsync(undoPlan, localization.Format("Status.Renaming", count.ToString(CultureInfo.InvariantCulture)));

        if (result.Success)
        {
            _lastExecutedPlan = null;
            StatusText = localization.Format("Status.Undone", result.Completed.ToString(CultureInfo.InvariantCulture));
            StatusLevel = StatusLevel.Success;
            DialogService.Success(StatusText);
        }
        else
        {
            ReportFailure(result);
        }

        OnPropertyChanged(nameof(CanUndo));
        await RefreshFolderAsync();
    }

    /// <summary>Stops a running rename; the executor restores every file it already touched.</summary>
    public void Cancel()
    {
        _cancellation?.Cancel();
        StatusText = LocalizationService.Instance.Get("Status.Cancelled");
        StatusLevel = StatusLevel.Warning;
    }

    /// <summary>Checks GitHub for a newer release. Silent checks only update the banner.</summary>
    public async Task CheckForUpdatesAsync(bool silent)
    {
        var localization = LocalizationService.Instance;

        if (!silent)
        {
            StatusText = localization.Get("Settings.Checking");
            StatusLevel = StatusLevel.Info;
        }

        var result = await _updateService
            .CheckAsync(_settings.RepositoryOwner, _settings.RepositoryName)
            .ConfigureAwait(true);

        _latestReleaseUrl = result.ReleaseUrl;

        switch (result.Status)
        {
            case UpdateStatus.Available:
                IsUpdateAvailable = true;
                UpdateText = localization.Format("Settings.NewVersion", result.Version ?? string.Empty);
                if (!silent)
                {
                    DialogService.Info(UpdateText);
                }

                break;

            case UpdateStatus.UpToDate:
                IsUpdateAvailable = false;
                if (!silent)
                {
                    StatusText = localization.Get("Settings.UpToDate");
                    StatusLevel = StatusLevel.Success;
                    DialogService.Info(StatusText);
                }

                break;

            case UpdateStatus.NoRelease:
                IsUpdateAvailable = false;
                if (!silent)
                {
                    DialogService.Info(localization.Get("Settings.UpToDate"));
                }

                break;

            default:
                IsUpdateAvailable = false;
                if (!silent)
                {
                    DialogService.Warn(localization.Get("Settings.UpdateFailed"));
                }

                break;
        }
    }

    /// <summary>Opens the release page of the newest version, or the repository when unknown.</summary>
    public void OpenReleasePage()
    {
        var url = _latestReleaseUrl ?? AppInfo.ReleasesUrl;
        OpenInBrowser(url);
    }

    public void OpenFolder()
    {
        if (!HasFolder)
        {
            DialogService.Warn(LocalizationService.Instance.Get("Msg.NoFolder"));
            return;
        }

        OpenInBrowser(FolderPath);
    }

    /// <summary>Remembers the naming choices so the next run starts where the user stopped.</summary>
    public void PersistNamingSettings()
    {
        _settings.PatternPreset = Preset.ToString();
        _settings.CustomPrefix = CustomPrefix;
        _settings.StartNumber = StartNumber;
        _settings.Digits = Digits;
        _settingsService.Save(_settings);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;
        _cancellation?.Dispose();
    }

    // ---------------------------------------------------------------- internals

    public NamingPattern BuildPattern()
    {
        var prefix = Preset switch
        {
            NamingPreset.File => NamingPattern.File.Prefix,
            NamingPreset.Custom => CustomPrefix,
            _ => NamingPattern.Image.Prefix,
        };

        return new NamingPattern(prefix, " ", StartNumber, Digits);
    }

    private async Task<RenameResult> ExecuteAsync(RenamePlan plan, string runningStatus)
    {
        IsBusy = true;
        ProgressValue = 0;
        StatusText = runningStatus;
        StatusLevel = StatusLevel.Info;

        _cancellation?.Dispose();
        _cancellation = new CancellationTokenSource();
        var token = _cancellation.Token;

        var progress = new Progress<RenameProgress>(report =>
        {
            ProgressValue = report.OverallPercent;
        });

        try
        {
            return await Task.Run(() => _executor.Execute(plan, progress, token), token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return new RenameResult { Success = false, Cancelled = true, RolledBack = true };
        }
        catch (Exception exception)
        {
            return new RenameResult
            {
                Success = false,
                Errors = [new RenameError(string.Empty, exception.Message)],
            };
        }
        finally
        {
            IsBusy = false;
            ProgressValue = 0;
        }
    }

    private void ApplyPlan(RenamePlan plan, NamingPattern pattern)
    {
        _plan = plan;
        _planKey = BuildPlanKey(pattern);

        IsPreviewTruncated = plan.Items.Count > MaxPreviewRows;
        Rows = plan.Items
            .Take(MaxPreviewRows)
            .Select(item => new PreviewRow(item.SourceFileName, item.TargetFileName, !item.IsNoOp))
            .ToList();

        OnPropertyChanged(nameof(PreviewSummaryText));

        var localization = LocalizationService.Instance;
        if (plan.HasConflicts)
        {
            StatusText = localization.Format("Status.Conflicts", plan.Conflicts.Count.ToString(CultureInfo.InvariantCulture));
            StatusLevel = StatusLevel.Error;
        }
        else if (plan.ChangeCount == 0)
        {
            StatusText = localization.Get("Status.NothingToDo");
            StatusLevel = StatusLevel.Warning;
        }
        else
        {
            StatusText = localization.Format("Status.PreviewReady", plan.ChangeCount.ToString(CultureInfo.InvariantCulture));
            StatusLevel = StatusLevel.Success;
        }
    }

    private void ClearPreview()
    {
        _plan = null;
        _planKey = string.Empty;
        IsPreviewTruncated = false;
        Rows = Array.Empty<PreviewRow>();
        OnPropertyChanged(nameof(PreviewSummaryText));
    }

    private void ReportFailure(RenameResult result)
    {
        var localization = LocalizationService.Instance;
        var details = string.Join(
            Environment.NewLine,
            result.Errors.Select(error => string.IsNullOrEmpty(error.FileName)
                ? error.Message
                : $"{error.FileName}: {error.Message}"));

        if (result.Cancelled)
        {
            StatusText = localization.Get("Status.Cancelled");
            StatusLevel = StatusLevel.Warning;
        }
        else
        {
            StatusText = localization.Get("Status.Failed");
            StatusLevel = StatusLevel.Error;
        }

        if (details.Length > 0)
        {
            DialogService.Error(details);
        }
    }

    private string BuildPlanKey(NamingPattern pattern) =>
        string.Join(
            '|',
            FolderPath,
            _settings.IncludeSubfolders ? "deep" : "flat",
            pattern.Prefix,
            pattern.StartNumber.ToString(CultureInfo.InvariantCulture),
            pattern.Padding.ToString(CultureInfo.InvariantCulture));

    private static string FormatSeconds(TimeSpan duration) =>
        duration.TotalSeconds.ToString("0.0", CultureInfo.InvariantCulture);

    private static NamingPreset ParsePreset(string? value) =>
        Enum.TryParse<NamingPreset>(value, ignoreCase: true, out var preset) ? preset : NamingPreset.Image;

    private static void OpenInBrowser(string target)
    {
        try
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            DialogService.Error(exception.Message);
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        OnPropertiesChanged(
            nameof(FolderDisplay),
            nameof(FileCountText),
            nameof(ExampleText),
            nameof(PreviewSummaryText),
            nameof(TruncatedText),
            nameof(UpdateText));

        // Rebuild the current preview text so conflicts and counts use the new language.
        if (_plan is not null)
        {
            ApplyPlan(_plan, BuildPattern());
        }
    }
}
