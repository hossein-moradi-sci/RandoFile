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
    public void The_intro_is_long_enough_to_be_watched()
    {
        // The splash may only leave once the whole intro has played, otherwise the last beat is cut
        // off half way and the screen reads as a flicker instead of an opening.
        var floor = MinimumDurationInSeconds();
        Assert.True(floor >= 2.5, $"The splash is only held for {floor} s, which is too brief to follow.");

        var intro = Splash.Value
            .Descendants()
            .Where(element => element.Name.LocalName == "Storyboard")
            .Where(element => element.Attribute(XamlNamespace + "Key") is not null)
            .Select(element => (Name: element.Attribute(XamlNamespace + "Key")!.Value, Element: element))
            // The endless loops never finish, so they say nothing about how long the intro runs.
            .Where(storyboard => !storyboard.Name.EndsWith("Loop", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(intro);

        var unfinished = intro
            .Select(storyboard => (storyboard.Name, Seconds: FinishesAt(storyboard.Element)))
            .Where(entry => entry.Seconds > floor)
            .OrderBy(entry => entry.Seconds)
            .ToList();

        Assert.True(
            unfinished.Count == 0,
            "The splash closes before these have finished: " +
            string.Join(", ", unfinished.Select(entry => $"{entry.Name} at {entry.Seconds:0.00} s")));
    }

    [Fact]
    public void The_credit_stays_up_long_enough_to_read_it()
    {
        // The whole point of the line is that someone reads it. It has to arrive well before the
        // splash closes and then sit there, not flash past in the last few hundred milliseconds.
        var floor = MinimumDurationInSeconds();

        var settled = Splash.Value
            .Descendants()
            .Where(element => element.Name.LocalName == "Storyboard")
            .Select(element => element.Attribute(XamlNamespace + "Key")?.Value)
            .Where(key => key is not null)
            .Select(key => key!)
            .ToList();

        var credit = settled
            .Where(key => !key.EndsWith("Loop", StringComparison.Ordinal))
            .Select(key => Splash.Value.Descendants().Single(element =>
                element.Name.LocalName == "Storyboard" &&
                element.Attribute(XamlNamespace + "Key")?.Value == key))
            .Select(storyboard => new
            {
                Key = storyboard.Attribute(XamlNamespace + "Key")!.Value,
                Seconds = storyboard.Elements()
                    .Where(child => child.Attribute("Duration") is not null &&
                                    (child.Attribute("Storyboard.TargetName")?.Value ?? string.Empty).StartsWith("Credit", StringComparison.Ordinal))
                    .Select(child => Seconds(storyboard.Attribute("BeginTime")?.Value) +
                                     Seconds(child.Attribute("BeginTime")?.Value) +
                                     Seconds(child.Attribute("Duration")!.Value))
                    .DefaultIfEmpty(0)
                    .Max(),
            })
            .Where(entry => entry.Seconds > 0)
            .OrderByDescending(entry => entry.Seconds)
            .First();

        var readable = floor - credit.Seconds;
        Assert.True(
            readable >= 1.5,
            $"The credit line is only on screen for {readable:0.00} s after it settles at {credit.Seconds:0.00} s; " +
            "nobody can read it that fast.");
    }

    [Fact]
    public void The_credit_line_follows_the_interface_language()
    {
        // The splash is built before the language is applied, so the text has to come from the
        // resource dictionary; a hard coded name would always read English.
        Assert.Equal("{DynamicResource Splash.Credit}", Find("TextBlock", "CreditText").Attribute("Text")?.Value);

        // It is framed and glowing, not a bare line of text: a plain TextBlock was rejected because
        // it had no visible corners, no effect and did not stand out on the dark card.
        var frame = Find("Border", "CreditFrame");
        Assert.Equal("18", frame.Attribute("CornerRadius")?.Value);
        Assert.Equal("1.4", frame.Attribute("BorderThickness")?.Value);
        Assert.NotNull(frame.Attribute("BorderBrush"));

        var glow = frame.Elements()
            .Where(element => element.Name.LocalName == "Border.Effect")
            .SelectMany(element => element.Elements())
            .SingleOrDefault(element => element.Name.LocalName == "DropShadowEffect");

        Assert.True(glow is not null, "The credit frame has no glow at all.");

        var colour = (System.Windows.Media.Color)System.Windows.Media.ColorConverter
            .ConvertFromString(glow!.Attribute("Color")!.Value)!;

        Assert.True(
            colour.R + colour.G + colour.B > 200,
            $"The glow colour {colour} is not neon, so the frame does not light up.");

        Assert.True(
            double.Parse(glow.Attribute("BlurRadius")!.Value, System.Globalization.CultureInfo.InvariantCulture) >= 8,
            "The glow is too tight to read as a glow.");

        Assert.True(
            double.Parse(glow.Attribute("Opacity")!.Value, System.Globalization.CultureInfo.InvariantCulture) >= 0.4,
            "The glow is switched off.");

        foreach (var corner in new[] { "CreditTickTopLeft", "CreditTickTopRight", "CreditTickBottomLeft", "CreditTickBottomRight" })
        {
            var tick = Find("Path", corner);

            Assert.True(tick.Attribute("Data")!.Value.Contains('A'), $"'{corner}' is drawn as a plain line, not a corner.");
            Assert.True(
                double.Parse(tick.Attribute("StrokeThickness")!.Value, System.Globalization.CultureInfo.InvariantCulture) >= 1.5,
                $"'{corner}' is too thin to see.");
        }

        foreach (var language in new[] { "en", "fa", "fr", "ar" })
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Strings", $"Strings.{language}.xaml");
            Assert.True(File.Exists(path), $"Missing resource file: {path}");

            var keys = XDocument.Load(path)
                .Descendants()
                .Attributes(XamlNamespace + "Key")
                .Select(attribute => attribute.Value)
                .ToHashSet(StringComparer.Ordinal);

            Assert.Contains("Splash.Credit", keys);
        }
    }

    [Fact]
    public void Every_resource_the_splash_asks_for_exists()
    {
        var available = XDocument.Load(Path.Combine(AppContext.BaseDirectory, "Strings", "Strings.en.xaml"))
            .Descendants()
            .Attributes(XamlNamespace + "Key")
            .Select(attribute => attribute.Value)
            .ToHashSet(StringComparer.Ordinal);

        var requested = System.Text.RegularExpressions.Regex
            .Matches(Splash.Value.ToString(), @"\{DynamicResource\s+([A-Za-z0-9_.]+)\}")
            .Select(match => match.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Assert.NotEmpty(requested);

        var unknown = requested.Where(key => !available.Contains(key)).OrderBy(key => key, StringComparer.Ordinal).ToList();
        Assert.True(unknown.Count == 0, "The splash asks for strings that no language defines: " + string.Join(", ", unknown));
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

    /// <summary>The moment the last beat of a storyboard has landed, in seconds.</summary>
    private static double FinishesAt(XElement storyboard)
    {
        var start = Seconds(storyboard.Attribute("BeginTime")?.Value);

        return storyboard.Elements()
            .Where(child => child.Attribute("Duration") is not null)
            .Select(child => start + Seconds(child.Attribute("BeginTime")?.Value) + Seconds(child.Attribute("Duration")!.Value))
            .DefaultIfEmpty(start)
            .Max();
    }

    /// <summary>Reads a WPF time span such as <c>0:0:1.62</c> or a bare number of seconds.</summary>
    private static double Seconds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        var parts = value.Split(':');

        return parts.Length switch
        {
            1 => double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
            3 => double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
            _ => throw new FormatException($"'{value}' is not a time span the splash can use."),
        };
    }

    /// <summary>The floor the code keeps the splash on screen for, in seconds.</summary>
    private static double MinimumDurationInSeconds()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Code", "SplashWindow.xaml.cs");
        Assert.True(File.Exists(path), $"Missing source file: {path}");

        var match = System.Text.RegularExpressions.Regex.Match(
            File.ReadAllText(path),
            @"MinimumDuration\s*=\s*TimeSpan\.FromMilliseconds\((\d+)\)");

        Assert.True(match.Success, "SplashWindow.xaml.cs no longer declares MinimumDuration in milliseconds.");

        return double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture) / 1000d;
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
