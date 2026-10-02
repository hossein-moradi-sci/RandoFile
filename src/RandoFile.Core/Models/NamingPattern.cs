using System.Globalization;
using System.Text;

namespace RandoFile.Core.Models;

/// <summary>
/// Describes how a new file name is composed: <c>{Prefix}{Separator}{Number}{Extension}</c>.
/// </summary>
/// <param name="Prefix">Base word, for example <c>Image</c> or <c>File</c>.</param>
/// <param name="Separator">Text placed between the prefix and the number.</param>
/// <param name="StartNumber">Number used for the first file of the batch.</param>
/// <param name="Padding">Minimum digits for the number (0 disables zero padding).</param>
public sealed record NamingPattern(
    string Prefix,
    string Separator = " ",
    int StartNumber = 1,
    int Padding = 0)
{
    /// <summary>Preset producing <c>Image 1.jpg</c>, <c>Image 2.jpg</c>, ...</summary>
    public static NamingPattern Image { get; } = new("Image");

    /// <summary>Preset producing <c>File 1.txt</c>, <c>File 2.txt</c>, ...</summary>
    public static NamingPattern File { get; } = new("File");

    /// <summary>Builds the file name for the item at <paramref name="index"/> inside the batch.</summary>
    public string BuildName(int index, string? extension)
    {
        var number = StartNumber + index;
        var digits = number.ToString(CultureInfo.InvariantCulture);
        if (Padding > 0)
        {
            digits = digits.PadLeft(Padding, '0');
        }

        var prefix = Sanitize(Prefix);
        var body = prefix.Length == 0 ? digits : prefix + Separator + digits;
        return body + NormalizeExtension(extension);
    }

    /// <summary>Validates the pattern and returns human readable problems (empty when the pattern is usable).</summary>
    public IReadOnlyList<string> Validate()
    {
        var problems = new List<string>();

        if (Sanitize(Prefix).Length == 0 && Prefix.Trim().Length > 0)
        {
            problems.Add("The prefix contains only characters that are not allowed in a file name.");
        }

        if (StartNumber < 0)
        {
            problems.Add("The start number cannot be negative.");
        }

        if (Padding is < 0 or > 12)
        {
            problems.Add("The number of digits must be between 0 and 12.");
        }

        if (Sanitize(Separator) != Separator.Trim())
        {
            problems.Add("The separator contains characters that are not allowed in a file name.");
        }

        return problems;
    }

    /// <summary>Removes characters that Windows forbids inside a file name.</summary>
    public static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (Array.IndexOf(invalid, character) < 0 && char.IsControl(character) == false)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Trim();
    }

    /// <summary>Ensures the extension is prefixed with a dot; returns an empty string when there is none.</summary>
    public static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return string.Empty;
        }

        var trimmed = extension.Trim();
        return trimmed[0] == '.' ? trimmed : "." + trimmed;
    }

}
