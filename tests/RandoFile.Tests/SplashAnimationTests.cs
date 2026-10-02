using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace RandoFile.Core.Tests;

/// <summary>
/// Guards the splash screen. It is built from vectors and storyboards, and both kinds of mistake are
/// invisible until you watch it: a <c>Storyboard.TargetName</c> that matches no <c>x:Name</c> fails
/// silently and simply does not animate, and a malformed <c>Path.Data</c> throws while the window is
/// being created. The test project uses WPF, so the geometry is parsed here the way the runtime parses it.
/// </summary>
public class SplashAnimationTests
{
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    private static readonly Lazy<XDocument> Splash = new(() => Load("Xaml", "SplashWindow.xaml"));

    [Fact]
    public void Every_animated_element_actually_exists()
    {
        var names = Splash.Value
            .Descendants()
            .Attributes(XamlNamespace + "Name")
            .Select(attribute => attribute.Value)
            .ToHashSet(StringComparer.Ordinal);

        var missing = Splash.Value
            .Descendants()
            .Attributes()
            // Written as "Storyboard.TargetName", which is one XML name containing a dot.
            .Where(attribute => attribute.Name.LocalName.EndsWith("TargetName", StringComparison.Ordinal))
            .Select(attribute => attribute.Value)
            .Distinct(StringComparer.Ordinal)
            .Where(target => !names.Contains(target))
            .OrderBy(target => target, StringComparer.Ordinal)
            .ToList();

        Assert.NotEmpty(names);
        Assert.True(
            missing.Count == 0,
            "These storyboards animate an element that does not exist, so they do nothing: " + string.Join(", ", missing));
    }

    [Fact]
    public void The_fan_is_drawn_from_three_cards_and_a_ring()
    {
        // Two side cards plus the hero card, each a body and a fold, under one ring arc.
        AssertSize("LeftCard", 61, 75);
        AssertSize("RightCard", 61, 75);
        AssertSize("HeroCard", 67, 86);

        AssertSize("LeftFold", 18, 18);
        AssertSize("RightFold", 18, 18);
        AssertSize("HeroFold", 20, 20);

        // The arc is shared by a wide translucent halo and the gradient stroke on top of it.
        AssertSize("RingHalo", 114, 31);
        AssertSize("RingArc", 114, 31);
        AssertSize("RingHead", 14, 16);
    }

    [Fact]
    public void The_side_cards_rest_at_opposing_angles()
    {
        // The fan is only a fan because the side cards lean away from the hero card.
        Assert.Equal(-26, Tilt("LeftTilt"));
        Assert.Equal(26, Tilt("RightTilt"));
        Assert.Equal(0, Tilt("HeroTilt"));
    }

    [Fact]
    public void Every_brush_literal_in_the_markup_can_actually_be_parsed()
    {
        // WPF accepts Fill="None" when it reads loose XAML, but the compiled BAML path hands the
        // token to Brush.Parse, which rejects it and takes the whole window down at start-up.
        // Nothing about that shows at build time, so every literal in the project is parsed here.
        var problems = new List<string>();
        var scanned = 0;

        foreach (var folder in new[] { "Xaml", "Themes", "Help" })
        {
            var directory = Path.Combine(AppContext.BaseDirectory, folder);
            if (!Directory.Exists(directory))
            {
                continue;
            }

            foreach (var file in Directory.GetFiles(directory, "*.xaml").OrderBy(name => name, StringComparer.Ordinal))
            {
                scanned++;

                foreach (var attribute in XDocument.Load(file).Descendants().Attributes())
                {
                    if (attribute.Name.LocalName is not ("Fill" or "Stroke" or "Background" or "BorderBrush" or "Foreground" or "Color"))
                    {
                        continue;
                    }

                    var value = attribute.Value;
                    if (value.Length == 0 || value.StartsWith('{') || value.StartsWith("$", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    try
                    {
                        _ = new BrushConverter().ConvertFromString(value);
                    }
                    catch (Exception)
                    {
                        problems.Add($"{Path.GetFileName(file)}: {attribute.Name.LocalName}=\"{value}\" is not a brush WPF can parse");
                    }
                }
            }
        }

        Assert.True(scanned > 0, "No markup was found next to the tests.");
        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    /// <summary>Looks a named <c>Path</c> up by <c>x:Name</c> and checks the size of its geometry.</summary>
    private static void AssertSize(string name, double width, double height)
    {
        var path = Find("Path", name);
        var data = path.Attribute("Data");
        Assert.True(data is not null, $"'{name}' has no Data.");

        var bounds = Geometry.Parse(data!.Value).Bounds;
        Assert.True(
            Math.Abs(bounds.Width - width) < 1.5 && Math.Abs(bounds.Height - height) < 1.5,
            $"'{name}' should be {width}x{height} but measures {bounds.Width}x{bounds.Height}.");
    }

    /// <summary>The rest angle of a named <c>RotateTransform</c>.</summary>
    private static double Tilt(string name) => double.Parse(
        Find("RotateTransform", name).Attribute("Angle")!.Value,
        System.Globalization.CultureInfo.InvariantCulture);

    private static XElement Find(string element, string name)
    {
        var match = Splash.Value
            .Descendants()
            .Where(candidate => candidate.Name.LocalName == element)
            .FirstOrDefault(candidate => candidate.Attribute(XamlNamespace + "Name")?.Value == name);

        Assert.True(match is not null, $"The splash has no {element} named '{name}'.");
        return match!;
    }

    private static XDocument Load(string folder, string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, folder, fileName);
        Assert.True(File.Exists(path), $"Missing markup file: {path}");
        return XDocument.Load(path);
    }
}
