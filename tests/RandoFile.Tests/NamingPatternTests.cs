using RandoFile.Core.Models;

namespace RandoFile.Core.Tests;

public class NamingPatternTests
{
    [Fact]
    public void Image_preset_produces_sequential_names()
    {
        var pattern = NamingPattern.Image;

        Assert.Equal("Image 1.jpg", pattern.BuildName(0, ".jpg"));
        Assert.Equal("Image 2.jpg", pattern.BuildName(1, ".jpg"));
        Assert.Equal("Image 10.jpg", pattern.BuildName(9, ".jpg"));
    }

    [Fact]
    public void File_preset_uses_the_file_prefix()
    {
        Assert.Equal("File 1.txt", NamingPattern.File.BuildName(0, ".txt"));
    }

    [Theory]
    [InlineData(3, "Image 001.png")]
    [InlineData(5, "Image 00001.png")]
    [InlineData(0, "Image 1.png")]
    public void Padding_adds_leading_zeros(int padding, string expected)
    {
        var pattern = new NamingPattern("Image", " ", 1, padding);

        Assert.Equal(expected, pattern.BuildName(0, ".png"));
    }

    [Fact]
    public void Start_number_offsets_the_sequence()
    {
        var pattern = new NamingPattern("Image", " ", 100);

        Assert.Equal("Image 100.jpg", pattern.BuildName(0, ".jpg"));
        Assert.Equal("Image 101.jpg", pattern.BuildName(1, ".jpg"));
    }

    [Theory]
    [InlineData("jpg", "Image 1.jpg")]
    [InlineData(".jpg", "Image 1.jpg")]
    [InlineData("", "Image 1")]
    [InlineData(null, "Image 1")]
    public void Extension_is_preserved_and_normalised(string? extension, string expected)
    {
        Assert.Equal(expected, NamingPattern.Image.BuildName(0, extension));
    }

    [Fact]
    public void Invalid_characters_are_removed_from_the_prefix()
    {
        var pattern = NamingPattern.Image with { Prefix = "My/Holiday:2026" };

        Assert.Equal("MyHoliday2026 1.jpg", pattern.BuildName(0, ".jpg"));
    }

    [Fact]
    public void An_empty_prefix_produces_a_plain_number()
    {
        var pattern = new NamingPattern(string.Empty);

        Assert.Equal("1.jpg", pattern.BuildName(0, ".jpg"));
    }

    [Fact]
    public void A_prefix_made_only_of_invalid_characters_is_reported()
    {
        var pattern = NamingPattern.Image with { Prefix = "***" };

        // "***" is stripped away completely, which is worth telling the user about,
        // but the file name itself is still perfectly valid.
        Assert.NotEmpty(pattern.Validate());
        Assert.Equal("1.jpg", pattern.BuildName(0, ".jpg"));
    }

    [Fact]
    public void A_normal_pattern_validates_cleanly()
    {
        Assert.Empty(NamingPattern.Image.Validate());
        Assert.Empty(new NamingPattern("Holiday 2026", "-", 5, 4).Validate());
    }

    [Fact]
    public void Out_of_range_options_are_rejected()
    {
        Assert.NotEmpty(new NamingPattern("Image", " ", 1, 99).Validate());
        Assert.NotEmpty(new NamingPattern("Image", " ", -5, 0).Validate());
    }
}
