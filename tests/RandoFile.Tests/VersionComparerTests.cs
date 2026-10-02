using RandoFile.Core.Services;

namespace RandoFile.Core.Tests;

public class VersionComparerTests
{
    [Theory]
    [InlineData("v1.0.1", "1.0.0", true)]
    [InlineData("2.0.0", "1.9.9", true)]
    [InlineData("1.1", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("v1.0.0", "1.0.0", false)]
    [InlineData("0.9.0", "1.0.0", false)]
    public void Newer_versions_are_detected(string candidate, string current, bool expected)
    {
        Assert.Equal(expected, VersionComparer.IsNewer(candidate, current));
    }

    [Theory]
    [InlineData("v1.2.0-beta.1", "1.2.0")]
    [InlineData("1.2", "1.2.0")]
    [InlineData("V1.2.0", "v1.2.0")]
    public void Tags_are_normalised_before_comparing(string left, string right)
    {
        Assert.False(VersionComparer.IsNewer(left, right));
        Assert.False(VersionComparer.IsNewer(right, left));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("nightly")]
    public void Unreadable_tags_never_look_newer(string? tag)
    {
        Assert.False(VersionComparer.IsNewer(tag, "1.0.0"));
        Assert.False(VersionComparer.IsNewer("1.0.0", tag));
    }

    [Fact]
    public void Normalise_expands_short_versions()
    {
        Assert.Equal("1.2.0", VersionComparer.Normalize("v1.2"));
        Assert.Equal("3.0.0", VersionComparer.Normalize("3"));
        Assert.Equal("1.2.3", VersionComparer.Normalize("v1.2.3"));
    }
}
