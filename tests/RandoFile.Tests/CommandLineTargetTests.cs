using System;
using System.IO;
using RandoFile.Core;

namespace RandoFile.Core.Tests;

/// <summary>
/// Covers the path Windows hands the program when it is opened from a file association, a
/// shortcut or "Open with". Every association the installer registers launches
/// <c>RandoFile.exe "%1"</c>, so this is the code that decides whether the program honours the
/// user's choice or opens and silently ignores it.
/// </summary>
public class CommandLineTargetTests
{
    [Fact]
    public void A_folder_argument_is_the_folder_to_open()
    {
        using var temp = new TempFolder();

        Assert.Equal(Path.GetFullPath(temp.Root), CommandLineTarget.ResolveFolder(temp.Root));
    }

    [Fact]
    public void A_file_argument_opens_the_folder_that_file_is_in()
    {
        // RandoFile randomizes the contents of a folder, so opening a file means "this file's
        // folder", not "this file".
        using var temp = new TempFolder();
        var file = temp.AddFile("holiday.jpg");

        Assert.Equal(Path.GetFullPath(temp.Root), CommandLineTarget.ResolveFolder(file));
    }

    [Fact]
    public void Quoting_from_the_shell_is_not_the_caller_s_problem()
    {
        using var temp = new TempFolder();

        Assert.Equal(Path.GetFullPath(temp.Root), CommandLineTarget.ResolveFolder('"' + temp.Root + '"'));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\"\"")]
    public void Nothing_to_open_is_not_an_error(string? argument)
    {
        Assert.Null(CommandLineTarget.ResolveFolder(argument));
    }

    [Fact]
    public void A_path_that_no_longer_exists_is_ignored()
    {
        // A shell verb can be run long after the file it was given has been moved or deleted.
        // Popping a dialog about that at startup would be worse than doing nothing.
        using var temp = new TempFolder();
        var missing = temp.PathOf(Path.Combine("gone", "and", "deleted.txt"));

        Assert.Null(CommandLineTarget.ResolveFolder(missing));
    }

    [Fact]
    public void A_path_that_is_not_a_path_at_all_is_ignored()
    {
        Assert.Null(CommandLineTarget.ResolveFolder("C:\\<>|\":not a folder"));
    }

    [Fact]
    public void Switches_are_skipped_and_the_first_real_folder_wins()
    {
        using var first = new TempFolder();
        using var second = new TempFolder();

        var folder = CommandLineTarget.ResolveFolderFromArguments(
            new[] { "--some-future-switch", first.Root, second.Root });

        Assert.Equal(Path.GetFullPath(first.Root), folder);
    }

    [Fact]
    public void A_command_line_with_no_usable_path_leaves_the_previous_folder_alone()
    {
        Assert.Null(CommandLineTarget.ResolveFolderFromArguments(null));
        Assert.Null(CommandLineTarget.ResolveFolderFromArguments(Array.Empty<string>()));
        Assert.Null(CommandLineTarget.ResolveFolderFromArguments(new[] { "-x" }));
    }
}