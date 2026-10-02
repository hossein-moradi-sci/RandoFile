using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandoFile.Core.Models;
using RandoFile.Core.Services;

namespace RandoFile.Core.Tests;

public class RenameExecutorTests
{
    [Fact]
    public void Every_file_is_renamed_and_keeps_its_extension()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("zzz.jpg", "first"),
            folder.AddFile("aaa.mp3", "second"),
            folder.AddFile("mmm.txt", "third"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image);
        var result = new RenameExecutor().Execute(plan);

        Assert.True(result.Success);
        Assert.Equal(3, result.Completed);
        Assert.Empty(result.Errors);

        var remaining = Directory.GetFiles(folder.Root).Select(Path.GetFileName).OrderBy(name => name).ToArray();

        Assert.Equal(3, remaining.Length);
        Assert.All(remaining, name => Assert.StartsWith("Image ", name));

        // The shuffle decides which file gets which number, but never touches an extension.
        Assert.Equal(
            new[] { ".jpg", ".mp3", ".txt" },
            remaining.Select(Path.GetExtension).OrderBy(extension => extension).ToArray());
    }

    [Fact]
    public void Swapping_two_names_is_safe_because_of_the_two_phase_rename()
    {
        using var folder = new TempFolder();
        var first = folder.AddFile("Image 1.jpg", "content-one");
        var second = folder.AddFile("Image 2.jpg", "content-two");

        var plan = new RenamePlan(
            folder.Root,
            new[]
            {
                new RenameItem(first, folder.PathOf("Image 2.jpg")),
                new RenameItem(second, folder.PathOf("Image 1.jpg")),
            });

        var result = new RenameExecutor().Execute(plan);

        Assert.True(result.Success);
        Assert.Equal("content-two", folder.Read("Image 1.jpg"));
        Assert.Equal("content-one", folder.Read("Image 2.jpg"));
    }

    [Fact]
    public void A_circular_three_way_rename_is_safe()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("Image 1.jpg", "one"),
            folder.AddFile("Image 2.jpg", "two"),
            folder.AddFile("Image 3.jpg", "three"),
        };

        var plan = new RenamePlan(
            folder.Root,
            new[]
            {
                new RenameItem(files[0], folder.PathOf("Image 2.jpg")),
                new RenameItem(files[1], folder.PathOf("Image 3.jpg")),
                new RenameItem(files[2], folder.PathOf("Image 1.jpg")),
            });

        var result = new RenameExecutor().Execute(plan);

        Assert.True(result.Success);
        Assert.Equal("three", folder.Read("Image 1.jpg"));
        Assert.Equal("one", folder.Read("Image 2.jpg"));
        Assert.Equal("two", folder.Read("Image 3.jpg"));
    }

    [Fact]
    public void A_missing_file_fails_the_run_and_restores_everything()
    {
        using var folder = new TempFolder();
        var existing = folder.AddFile("photo.jpg", "keep me");
        var missing = folder.PathOf("not-here.jpg");

        var plan = new RenamePlan(
            folder.Root,
            new[]
            {
                new RenameItem(existing, folder.PathOf("Image 1.jpg")),
                new RenameItem(missing, folder.PathOf("Image 2.jpg")),
            });

        var result = new RenameExecutor().Execute(plan);

        Assert.False(result.Success);
        Assert.True(result.RolledBack);
        Assert.NotEmpty(result.Errors);

        Assert.True(folder.Exists("photo.jpg"));
        Assert.False(folder.Exists("Image 1.jpg"));
        Assert.Equal("keep me", folder.Read("photo.jpg"));
    }

    [Fact]
    public void No_temporary_files_are_left_behind()
    {
        using var folder = new TempFolder();
        var files = Enumerable.Range(0, 12).Select(index => folder.AddFile($"f{index}.dat")).ToArray();

        new RenameExecutor().Execute(new RenamePlanBuilder().Build(files, NamingPattern.File));

        Assert.DoesNotContain(
            Directory.GetFiles(folder.Root),
            path => Path.GetFileName(path).StartsWith(RenameExecutor.TempFilePrefix, StringComparison.Ordinal));
    }

    [Fact]
    public void A_plan_with_conflicts_is_refused_without_touching_anything()
    {
        using var folder = new TempFolder();
        var source = folder.AddFile("photo.jpg");
        var occupied = folder.AddFile("Image 1.jpg", "untouchable");

        var plan = new RenamePlan(
            folder.Root,
            new[] { new RenameItem(source, occupied) },
            new[] { "Image 1.jpg" });

        var result = new RenameExecutor().Execute(plan);

        Assert.False(result.Success);
        Assert.False(result.RolledBack);
        Assert.Equal("untouchable", folder.Read("Image 1.jpg"));
        Assert.True(folder.Exists("photo.jpg"));
    }

    [Fact]
    public void Progress_is_reported_for_both_phases()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("a.tmp"),
            folder.AddFile("b.tmp"),
        };

        var reports = new List<RenameProgress>();
        var progress = new CollectingProgress(reports);

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.File);
        var result = new RenameExecutor().Execute(plan, progress);

        Assert.True(result.Success);
        Assert.Contains(reports, report => report.Stage == RenameStage.Staging);
        Assert.Contains(reports, report => report.Stage == RenameStage.Committing);
        Assert.Equal(100, reports.Max(report => report.OverallPercent));
    }

    [Fact]
    public void Undo_puts_every_original_name_back()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("holiday-1.jpg", "one"),
            folder.AddFile("holiday-2.jpg", "two"),
            folder.AddFile("holiday-3.jpg", "three"),
        };

        var executor = new RenameExecutor();
        var builder = new RenamePlanBuilder();

        var plan = builder.Build(files, NamingPattern.Image);
        Assert.True(executor.Execute(plan).Success);

        var undoPlan = builder.BuildUndo(plan);
        var undoResult = executor.Execute(undoPlan);

        Assert.True(undoResult.Success);
        Assert.Equal("one", folder.Read("holiday-1.jpg"));
        Assert.Equal("two", folder.Read("holiday-2.jpg"));
        Assert.Equal("three", folder.Read("holiday-3.jpg"));
    }

    [Fact]
    public void An_already_matching_folder_is_a_successful_no_op()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("Image 1.jpg"),
            folder.AddFile("Image 2.jpg"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image, shuffle: false);
        var result = new RenameExecutor().Execute(plan);

        Assert.True(result.Success);
        Assert.Equal(0, result.Completed);
        Assert.True(folder.Exists("Image 1.jpg"));
        Assert.True(folder.Exists("Image 2.jpg"));
    }

    /// <summary>Captures progress reports without needing a UI thread.</summary>
    private sealed class CollectingProgress : IProgress<RenameProgress>
    {
        private readonly List<RenameProgress> _reports;

        public CollectingProgress(List<RenameProgress> reports) => _reports = reports;

        public void Report(RenameProgress value) => _reports.Add(value);
    }
}
