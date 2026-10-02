using System.Windows;

namespace RandoFile.App.Services;

/// <summary>Small wrapper around the native message box so every dialog is translated and RTL aware.</summary>
public static class DialogService
{
    public static void Info(string message) => Show(message, L("Msg.InfoTitle"), MessageBoxImage.Information);

    public static void Success(string message) => Show(message, L("Msg.SuccessTitle"), MessageBoxImage.Information);

    public static void Warn(string message) => Show(message, L("Msg.WarningTitle"), MessageBoxImage.Warning);

    public static void Error(string message) => Show(message, L("Msg.ErrorTitle"), MessageBoxImage.Error);

    /// <summary>Yes/No confirmation; returns true when the user accepted.</summary>
    public static bool Confirm(string message, string title) =>
        ShowCore(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    private static void Show(string message, string title, MessageBoxImage icon) =>
        ShowCore(message, title, MessageBoxButton.OK, icon);

    private static MessageBoxResult ShowCore(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
    {
        // Persian and Arabic need the RTL reading flag, otherwise the text renders left aligned.
        var options = LocalizationService.Instance.IsRightToLeft
            ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign
            : MessageBoxOptions.None;

        var owner = Application.Current?.MainWindow;
        if (owner is not null && owner.IsLoaded)
        {
            return MessageBox.Show(owner, message, title, buttons, icon, MessageBoxResult.OK, options);
        }

        return MessageBox.Show(message, title, buttons, icon, MessageBoxResult.OK, options);
    }

    private static string L(string key) => LocalizationService.Instance.Get(key);
}
