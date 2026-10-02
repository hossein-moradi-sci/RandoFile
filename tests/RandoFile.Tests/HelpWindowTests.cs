using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RandoFile.Core.Models;

namespace RandoFile.Core.Tests;

/// <summary>
/// Guards the in-app help. Every reference in the help markup is a runtime failure waiting to
/// happen: a missing <c>StaticResource</c> throws while the window is being built, and a missing
/// <c>DynamicResource</c> silently renders an empty label. A translated key that nothing reads is
/// just maintenance debt, so that is caught here too.
/// </summary>
public class HelpWindowTests
{
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    private static readonly XNamespace StringNamespace = "clr-namespace:System;assembly=mscorlib";

    [Fact]
    public void Every_mockup_used_by_the_help_window_exists_and_is_used()
    {
        var defined = LoadMockupKeys().ToHashSet(StringComparer.Ordinal);

        var referenced = HelpMarkup()
            .SelectMany(entry => ReferencedKeys(entry.Document))
            .Where(key => key.StartsWith("Mockup.", StringComparison.Ordinal))
            .ToHashSet(StringComparer.Ordinal);

        Assert.NotEmpty(defined);
        Assert.NotEmpty(referenced);

        var missing = referenced.Except(defined).OrderBy(key => key, StringComparer.Ordinal).ToList();
        var unused = defined.Except(referenced).OrderBy(key => key, StringComparer.Ordinal).ToList();

        Assert.True(missing.Count == 0, "The help window asks for mock-ups that do not exist: " + string.Join(", ", missing));
        Assert.True(unused.Count == 0, "These mock-ups are never shown: " + string.Join(", ", unused));
    }

    [Fact]
    public void Every_resource_reference_in_the_help_markup_resolves()
    {
        // Either a translated string or a theme key; anything outside that set renders blank
        // or throws while the window is being built.
        var available = LoadStringKeys("en")
            .Concat(LoadThemeKeys("Light"))
            .Concat(LoadThemeKeys("Dark"))
            .Concat(LoadThemeKeys("Controls"))
            .Concat(LoadMockupKeys())
            .ToHashSet(StringComparer.Ordinal);

        var problems = HelpMarkup()
            .SelectMany(entry => ReferencedKeys(entry.Document)
                .Where(key => !available.Contains(key))
                .Select(key => $"{entry.Name}: unknown resource '{key}'"))
            .OrderBy(problem => problem, StringComparer.Ordinal)
            .ToList();

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void Every_translated_help_string_is_actually_shown()
    {
        var referenced = new[] { LoadView("MainWindow.xaml"), LoadView("SettingsWindow.xaml") }
            .Concat(HelpMarkup().Select(entry => entry.Document))
            .SelectMany(ReferencedKeys)
            .ToHashSet(StringComparer.Ordinal);

        var orphans = LoadStringKeys("en")
            .Where(key => key.StartsWith("Help.", StringComparison.Ordinal) || key == "Header.Help")
            .Where(key => !referenced.Contains(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToList();

        Assert.True(orphans.Count == 0, "Translated but never shown: " + string.Join(", ", orphans));
    }

    [Fact]
    public void The_help_markup_only_uses_alignment_values_wpf_understands()
    {
        // WPF has no logical "Start"/"End" alignment; those values only compile on other XAML
        // dialects and blow up at load time with a FormatException, long after the build passed.
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Left", "Center", "Right", "Stretch", "Top", "Bottom",
        };

        var problems = HelpMarkup()
            .SelectMany(entry => entry.Document.Descendants().Attributes()
                .Where(attribute => attribute.Name.LocalName is "HorizontalAlignment" or "VerticalAlignment")
                .Where(attribute => !allowed.Contains(attribute.Value))
                .Select(attribute => $"{entry.Name}: {attribute.Name.LocalName}=\"{attribute.Value}\" is not a WPF alignment value"))
            .ToList();

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void The_preset_words_the_help_promises_are_the_ones_the_app_writes()
    {
        // The guide tells the user that the Image and File presets stay in English whatever the
        // interface language is. That is only true while the presets keep their English prefixes.
        Assert.Equal("Image", NamingPattern.Image.Prefix);
        Assert.Equal("File", NamingPattern.File.Prefix);
        Assert.Equal("Image 1.jpg", NamingPattern.Image.BuildName(0, ".jpg"));
        Assert.Equal("File 3.txt", NamingPattern.File.BuildName(2, ".txt"));
    }

    /// <summary>The two markup files that make up the help, so every scan covers both.</summary>
    private static IEnumerable<(string Name, XDocument Document)> HelpMarkup()
    {
        yield return ("HelpWindow.xaml", LoadView("HelpWindow.xaml"));
        yield return ("Mockups.xaml", Load("Help", "Mockups.xaml"));
    }

    /// <summary>Every <c>{StaticResource …}</c> and <c>{DynamicResource …}</c> key used in an attribute value.</summary>
    private static IEnumerable<string> ReferencedKeys(XDocument document) =>
        document
            .Descendants()
            .Attributes()
            .SelectMany(attribute => Regex.Matches(
                attribute.Value,
                @"\{(?:Static|Dynamic)Resource\s+([A-Za-z0-9_.]+)\s*\}")
                .Select(match => match.Groups[1].Value))
            .Distinct(StringComparer.Ordinal);

    private static List<string> LoadMockupKeys() => Load("Help", "Mockups.xaml")
        .Root!
        .Elements()
        .Where(element => element.Name.LocalName == "DataTemplate")
        .Select(element => element.Attribute(XamlNamespace + "Key")?.Value)
        .Where(key => key is not null)
        .Select(key => key!)
        .ToList();

    private static List<string> LoadStringKeys(string language) => Load("Strings", $"Strings.{language}.xaml")
        .Root!
        .Elements(StringNamespace + "String")
        .Select(element => element.Attribute(XamlNamespace + "Key")?.Value)
        .Where(key => key is not null)
        .Select(key => key!)
        .ToList();

    private static List<string> LoadThemeKeys(string name) => Load("Themes", $"{name}.xaml")
        .Root!
        .Descendants()
        .Attributes(XamlNamespace + "Key")
        .Select(attribute => attribute.Value)
        .ToList();

    private static XDocument LoadView(string name) => Load("Xaml", name);

    private static XDocument Load(string folder, string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, folder, fileName);
        Assert.True(File.Exists(path), $"Missing markup file: {path}");
        return XDocument.Load(path);
    }
}
