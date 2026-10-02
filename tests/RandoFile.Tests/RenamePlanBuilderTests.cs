using System;
using System.IO;
using System.Linq;
using RandoFile.Core.Models;
using RandoFile.Core.Services;

namespace RandoFile.Core.Tests;

public class RenamePlanBuilderTests
{
    [Fact]
    public void Plan_keeps_every_extension_and_gives_every_file_a_unique_name()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("holiday.jpg"),
            folder.AddFile("song.mp3"),
            folder.AddFile("notes.txt"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image, shuffle: false);

        Assert.Empty(plan.Conflicts);
        Assert.Equal(3, plan.Items.Count);

        var targets = plan.Items.Select(item => item.TargetFileName).ToList();
        Assert.Equal(targets.Count, targets.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        Assert.EndsWith(".jpg", plan.Items[0].TargetFileName);
        Assert.EndsWith(".mp3", plan.Items[1].TargetFileName);
        Assert.EndsWith(".txt", plan.Items[2].TargetFileName);
    }

    [Fact]
    public void Every_target_stays_in_the_folder_of_its_source()
    {
        using var folder = new TempFolder();
        var nested = Directory.CreateDirectory(Path.Combine(folder.Root, "sub")).FullName;
        var files = new[]
        {
            folder.AddFile("one.jpg"),
            Path.Combine(nested, "two.jpg"),
        };
        File.WriteAllText(files[1], "x");

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image);

        Assert.All(plan.Items, item => Assert.Equal(item.Directory, Path.GetDirectoryName(item.TargetPath)));
        Assert.Empty(plan.Conflicts);
    }

    [Fact]
    public void Shuffling_changes_the_order_but_never_the_set_of_files()
    {
        using var folder = new TempFolder();
        var files = Enumerable.Range(0, 40).Select(index => folder.AddFile($"photo-{index}.jpg")).ToArray();

        var plan = new RenamePlanBuilder(new FileShuffler(seed: 1234)).Build(files, NamingPattern.Image);

        Assert.Equal(
            files.OrderBy(path => path, StringComparer.OrdinalIgnoreCase),
            plan.Items.Select(item => item.SourcePath).OrderBy(path => path, StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void A_target_held_by_a_file_outside_the_batch_is_a_conflict()
    {
        using var folder = new TempFolder();
        folder.AddFile("Image 1.jpg", "important");
        var files = new[]
        {
            folder.AddFile("photo-a.jpg"),
            folder.AddFile("photo-b.jpg"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image);

        Assert.True(plan.HasConflicts);
        Assert.Contains("Image 1.jpg", plan.Conflicts);
    }

    [Fact]
    public void Renaming_a_file_onto_another_batch_member_is_not_a_conflict()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("Image 1.jpg", "one"),
            folder.AddFile("Image 2.jpg", "two"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image);

        Assert.False(plan.HasConflicts);
        Assert.Empty(plan.Conflicts);
    }

    [Fact]
    public void A_folder_that_already_matches_the_pattern_only_produces_no_ops()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("Image 1.jpg"),
            folder.AddFile("Image 2.jpg"),
        };

        var plan = new RenamePlanBuilder().Build(files, NamingPattern.Image, shuffle: false);

        Assert.False(plan.HasConflicts);
        Assert.Equal(0, plan.ChangeCount);
    }

    [Fact]
    public void Undo_plan_swaps_the_source_and_target_of_every_item()
    {
        using var folder = new TempFolder();
        var files = new[]
        {
            folder.AddFile("a.jpg"),
            folder.AddFile("b.jpg"),
        };

        var builder = new RenamePlanBuilder();
        var plan = builder.Build(files, NamingPattern.Image, shuffle: false);

        // The undo plan is only valid once the rename actually happened.
        Assert.True(new RenameExecutor().Execute(plan).Success);
        var undo = builder.BuildUndo(plan);

        Assert.False(undo.HasConflicts);
        Assert.Equal(plan.Items.Count, undo.Items.Count);

        foreach (var item in plan.Items)
        {
            Assert.Contains(undo.Items, candidate => candidate.SourcePath == item.TargetPath && candidate.TargetPath == item.SourcePath);
        }
    }

    [Fact]
    public void An_empty_selection_produces_an_empty_plan()
    {
        var plan = new RenamePlanBuilder().Build(Array.Empty<string>(), NamingPattern.Image);

        Assert.True(plan.IsEmpty);
        Assert.False(plan.HasConflicts);
    }
}
