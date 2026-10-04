using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace RandoFile.App.Views;

/// <summary>
/// Animated brand screen shown while the main window is being created. It stays visible for at least
/// <see cref="MinimumDuration"/> so the intro never flashes past and the credit line at the bottom
/// stays up long enough to be read, then fades out and closes.
/// </summary>
public partial class SplashWindow : Window
{
    /// <summary>Lower bound for the intro; longer startup simply extends the splash.</summary>
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromMilliseconds(4500);

    /// <summary>
    /// The loops that never end on their own. They are kept so they can all be stopped before the
    /// window closes, otherwise their clocks keep the process alive a moment longer than needed.
    /// </summary>
    private static readonly string[] LoopKeys =
    [
        "RingLoop",
        "LeftCardLoop",
        "RightCardLoop",
        "HeroCardLoop",
        "ShimmerLoop",
        "TrackLoop",
        "CreditLoop",
    ];

    private readonly List<Storyboard> _loops = [];

    private DateTime _shownAt = DateTime.UtcNow;

    public SplashWindow()
    {
        InitializeComponent();

        // Resolved after InitializeComponent, because the storyboards live in Window.Resources.
        foreach (var key in LoopKeys)
        {
            _loops.Add((Storyboard)FindResource(key));
        }

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _shownAt = DateTime.UtcNow;

        BeginStoryboard((Storyboard)FindResource("FanIntro"));
        BeginStoryboard((Storyboard)FindResource("TextIntro"));

        foreach (var loop in _loops)
        {
            loop.Begin(this, isControllable: true);
        }
    }

    /// <summary>Waits out the remaining intro time, then fades the splash away and closes it.</summary>
    public async Task CloseAsync()
    {
        var remaining = MinimumDuration - (DateTime.UtcNow - _shownAt);
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining).ConfigureAwait(true);
        }

        // The endless loops must stop before the window closes, otherwise they keep the
        // animation clock alive and the process stays on screen for a moment longer than needed.
        foreach (var loop in _loops)
        {
            loop.Stop(this);
        }

        var outro = (Storyboard)FindResource("Outro");
        var finished = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnCompleted(object? sender, EventArgs e)
        {
            outro.Completed -= OnCompleted;
            finished.TrySetResult(true);
        }

        outro.Completed += OnCompleted;
        outro.Begin(this);

        await finished.Task.ConfigureAwait(true);
        Close();
    }
}
