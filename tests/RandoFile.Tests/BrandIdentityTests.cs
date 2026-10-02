using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RandoFile.Core;
using RandoFile.Core.Services;

namespace RandoFile.Core.Tests;

/// <summary>
/// Locks in the branding rules the project depends on: the brand is a compile-time constant and
/// must never appear in a language file, where a translator could translate or drop it.
/// </summary>
public class BrandIdentityTests
{
    private static readonly string[] Languages = ["en", "fa", "fr", "ar"];

    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    [Fact]
    public void The_brand_is_never_translated()
    {
        foreach (var language in Languages)
        {
            foreach (var (key, value) in LoadStrings(language))
            {
                Assert.DoesNotContain(
                    "App.Title",
                    key,
                    StringComparison.Ordinal);
                Assert.DoesNotContain(
                    AppInfo.Brand,
                    value,
                    StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void The_brand_is_a_single_readable_token()
    {
        // It has to survive a file name, a URL and a search query unchanged.
        Assert.Equal("RandoFile", AppInfo.Brand);
        Assert.DoesNotContain(' ', AppInfo.Brand);
        Assert.Matches("^[A-Za-z][A-Za-z0-9]*$", AppInfo.Brand);
    }

    [Fact]
    public void The_product_name_carries_the_maker_initials()
    {
        // "HM File Randomizer" is what makes the tool traceable to its author in a search,
        // while the short brand stays "RandoFile" everywhere inside the interface.
        Assert.Equal("HM File Randomizer", AppInfo.Name);
        Assert.NotEqual(AppInfo.Brand, AppInfo.Name);
    }

    [Fact]
    public void The_version_is_shown_with_the_documented_prefix()
    {
        Assert.Equal("v1.0.0", AppInfo.DisplayVersion);
        Assert.Matches(@"^\d+\.\d+\.\d+$", AppInfo.Version);
    }

    [Fact]
    public void The_staging_prefix_is_hidden_and_brand_scoped()
    {
        // Hidden so the user never sees it, and prefixed so it cannot collide with a real file.
        Assert.StartsWith(".", RenameExecutor.TempFilePrefix, StringComparison.Ordinal);
        Assert.Equal(".rf_tmp_", RenameExecutor.TempFilePrefix);
    }

    [Fact]
    public void The_repository_points_at_the_product_repository()
    {
        Assert.Equal("hossein-moradi-sci", AppInfo.RepositoryOwner);
        Assert.Equal("hm-file-randomizer", AppInfo.RepositoryName);
        Assert.Equal($"https://github.com/{AppInfo.RepositoryOwner}/{AppInfo.RepositoryName}", AppInfo.RepositoryUrl);
    }

    [Fact]
    public void The_creator_is_always_attributed()
    {
        Assert.Equal("Hossein Moradi", AppInfo.Creator);
        Assert.False(string.IsNullOrWhiteSpace(AppInfo.Tagline));
    }

    private static Dictionary<string, string> LoadStrings(string language)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Strings", $"Strings.{language}.xaml");
        Assert.True(File.Exists(path), $"Missing resource file: {path}");

        return XDocument.Load(path)
            .Root!
            .Elements()
            .ToDictionary(
                element => element.Attribute(XamlNamespace + "Key")!.Value,
                element => element.Value);
    }
}
