using System;
using System.Globalization;
using System.IO;
using System.Text;
using RandoFile.Core;

namespace RandoFile.App.Services;

/// <summary>
/// Appends fatal errors to <c>%LOCALAPPDATA%\RandoFile\crash.log</c>.
/// </summary>
/// <remarks>
/// The UI can only report a message in a dialog, which is useless when the very first frame
/// fails to build (a typical symptom of a broken publish). Writing the full exception to disk
/// keeps startup failures diagnosable outside a debugger.
/// </remarks>
public static class CrashLog
{
    private static readonly object Gate = new();

    public static string FilePath => Path.Combine(AppInfo.LogFolder, "crash.log");

    public static void Write(string stage, Exception exception)
    {
        try
        {
            lock (Gate)
            {
                Directory.CreateDirectory(AppInfo.LogFolder);
                var entry = new StringBuilder()
                    .AppendLine("---- " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " ----")
                    .AppendLine("stage: " + stage)
                    .AppendLine(exception.ToString())
                    .AppendLine();

                File.AppendAllText(FilePath, entry.ToString(), Encoding.UTF8);
            }
        }
        catch
        {
            // Logging must never be the reason the app dies.
        }
    }
}
