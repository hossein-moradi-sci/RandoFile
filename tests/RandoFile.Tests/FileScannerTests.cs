using System.IO;
using System.Linq;
using RandoFile.Core.Services;

namespace RandoFile.Core.Tests;

public class FileScannerTests
{
    [Fact]
    public void Every_file_type_is_picked_up()
    {
        using var folder = new TempFolder();
        folder.AddFile("photo.jpg");
        folder.AddFile("song.mp3");
        folder.AddFile("clip.mp4");
        folder.AddFile("notes.txt");
        folder.AddFile("archive.zip");

        var files = new FileScanner().Scan(folder.Root);

        Assert.Equal(5, files.Count);
    }

    [Fact]
    public void Subfolders_are_ignored_unless_asked_for()
    {
        using var folder = new TempFolder();
        folder.AddFile("top.jpg");

        var nested = Directory.CreateDirectory(Path.Combine(folder.Root, "sub")).FullName;
        File.WriteAllText(Path.Combine(nested, "deep.jpg"), "x");

        var scanner = new FileScanner();

        Assert.Single(scanner.Scan(folder.Root));
        Assert.Equal(2, scanner.Scan(folder.Root, includeSubfolders: true).Count);
    }

    [Fact]
    public void Leftovers_of_an_interrupted_rename_are_ignored()
    {
        using var folder = new TempFolder();
        folder.AddFile("photo.jpg");
        folder.AddFile(RenameExecutor.TempFilePrefix + "abc123.jpg");

        var files = new FileScanner().Scan(folder.Root);

        Assert.Single(files);
        Assert.EndsWith("photo.jpg", files[0]);
    }

    [Fact]
    public void A_missing_folder_yields_nothing_instead_of_throwing()
    {
        var files = new FileScanner().Scan(Path.Combine(Path.GetTempPath(), "ir-does-not-exist-" + "1234"));

        Assert.Empty(files);
    }

    [Fact]
    public void Results_are_sorted_so_repeated_scans_agree()
    {
        using var folder = new TempFolder();
        folder.AddFile("zeta.jpg");
        folder.AddFile("alpha.jpg");
        folder.AddFile("mid.jpg");

        var files = new FileScanner().Scan(folder.Root);

        Assert.Equal(files.OrderBy(path => path, System.StringComparer.OrdinalIgnoreCase), files);
    }
}
