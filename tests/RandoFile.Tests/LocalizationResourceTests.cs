using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RandoFile.App.Services;

namespace RandoFile.Core.Tests;

/// <summary>
/// Guards the resource dictionaries: a missing translation or a brush that only exists in the light
/// theme would otherwise only show up as an empty label at runtime.
/// </summary>
public class LocalizationResourceTests
{
    private static readonly string[] Languages = ["en", "fa", "fr", "ar"];

    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    private static readonly XNamespace StringNamespace = "clr-namespace:System;assembly=mscorlib";

    [Fact]
    public void A_fresh_installation_opens_in_english()
    {
        // English until the user says otherwise: nothing may guess a language from the machine.
        var settings = new AppSettings();

        Assert.Equal("en", settings.Language);
        Assert.Contains(settings.Language, Languages, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Every_language_defines_exactly_the_same_keys()
    {
        var reference = LoadStrings("en");

        Assert.NotEmpty(reference);

        foreach (var language in Languages)
        {
            var keys = LoadStrings(language);

            var missing = reference.Keys.Except(keys.Keys).OrderBy(key => key).ToList();
            var extra = keys.Keys.Except(reference.Keys).OrderBy(key => key).ToList();

            Assert.True(missing.Count == 0, $"{language} is missing: {string.Join(", ", missing)}");
            Assert.True(extra.Count == 0, $"{language} has unknown keys: {string.Join(", ", extra)}");
        }
    }

    [Fact]
    public void No_translation_is_left_empty()
    {
        foreach (var language in Languages)
        {
            foreach (var (key, value) in LoadStrings(language))
            {
                Assert.False(string.IsNullOrWhiteSpace(value), $"{language}: '{key}' is empty");
            }
        }
    }

    [Fact]
    public void Placeholders_match_between_languages()
    {
        var reference = LoadStrings("en");

        foreach (var language in Languages.Where(code => code != "en"))
        {
            foreach (var (key, value) in LoadStrings(language))
            {
                Assert.Equal(CountPlaceholders(reference[key]), CountPlaceholders(value));
            }
        }
    }

    [Fact]
    public void Both_themes_define_the_same_brushes()
    {
        var light = LoadBrushKeys("Light");
        var dark = LoadBrushKeys("Dark");

        Assert.NotEmpty(light);

        var missingInDark = light.Except(dark).OrderBy(key => key).ToList();
        var extraInDark = dark.Except(light).OrderBy(key => key).ToList();

        Assert.True(missingInDark.Count == 0, $"Dark theme is missing: {string.Join(", ", missingInDark)}");
        Assert.True(extraInDark.Count == 0, $"Dark theme has extra keys: {string.Join(", ", extraInDark)}");
    }

    [Fact]
    public void The_control_styles_resolve_every_brush_they_reference()
    {
        var available = LoadBrushKeys("Light")
            .Concat(LoadBrushKeys("Dark"))
            .ToHashSet(StringComparer.Ordinal);

        var controls = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Themes", "Controls.xaml"));

        var referenced = System.Text.RegularExpressions.Regex
            .Matches(controls, @"\{DynamicResource\s+(Brush\.[A-Za-z]+)\}")
            .Select(match => match.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Assert.NotEmpty(referenced);

        var unknown = referenced.Where(key => !available.Contains(key)).OrderBy(key => key).ToList();
        Assert.True(unknown.Count == 0, $"Unknown brushes: {string.Join(", ", unknown)}");
    }

    private static Dictionary<string, string> LoadStrings(string language)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Strings", $"Strings.{language}.xaml");
        Assert.True(File.Exists(path), $"Missing resource file: {path}");

        return XDocument.Load(path)
            .Root!
            .Elements(StringNamespace + "String")
            .ToDictionary(
                element => element.Attribute(XamlNamespace + "Key")!.Value,
                element => element.Value);
    }

    private static List<string> LoadBrushKeys(string theme)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Themes", $"{theme}.xaml");
        Assert.True(File.Exists(path), $"Missing theme file: {path}");

        return XDocument.Load(path)
            .Root!
            .Elements()
            .Select(element => element.Attribute(XamlNamespace + "Key")?.Value)
            .Where(key => key is not null)
            .Select(key => key!)
            .ToList();
    }

    private static int CountPlaceholders(string value) =>
        System.Text.RegularExpressions.Regex.Matches(value, @"\{\d+\}").Count;
}
