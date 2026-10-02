using System;
using System.IO;

namespace RandoFile.Core.Tests;

/// <summary>Creates a throwaway folder for one test and removes it afterwards.</summary>
internal sealed class TempFolder : IDisposable
{
    public TempFolder()
    {
        Root = Path.Combine(Path.GetTempPath(), "ir-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
    }

    /// <summary>Absolute path of the folder.</summary>
    public string Root { get; }

    /// <summary>Creates a file inside the folder and returns its full path.</summary>
    public string AddFile(string name, string content = "content")
    {
        var path = Path.Combine(Root, name);
        File.WriteAllText(path, content);
        return path;
    }

    public string PathOf(string name) => Path.Combine(Root, name);

    public bool Exists(string name) => File.Exists(PathOf(name));

    public string Read(string name) => File.ReadAllText(PathOf(name));

    public string[] Files() => Directory.GetFiles(Root);

    public void Dispose()
    {
        try
        {
            Directory.Delete(Root, recursive: true);
        }
        catch (IOException)
        {
            // A leftover temp folder is harmless for the test result.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
