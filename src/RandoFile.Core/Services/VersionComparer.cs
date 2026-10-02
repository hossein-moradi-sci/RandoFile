using System;
using System.Linq;

namespace RandoFile.Core.Services;

/// <summary>
/// Compares release tags such as <c>v1.2.0</c>, <c>1.2</c> or <c>2.0.0-beta.1</c>.
/// Kept in the core library so the update logic can be unit tested without the UI.
/// </summary>
public static class VersionComparer
{
    /// <summary>True when <paramref name="candidate"/> is a strictly higher version than <paramref name="current"/>.</summary>
    public static bool IsNewer(string? candidate, string? current)
    {
        var left = Parse(candidate);
        var right = Parse(current);

        return left is not null && right is not null && left > right;
    }

    /// <summary>True when both tags describe the same version.</summary>
    public static bool IsSame(string? left, string? right)
    {
        var first = Parse(left);
        var second = Parse(right);

        return first is not null && second is not null && first == second;
    }

    /// <summary>Strips the leading "v" and any pre-release suffix, then tries to read a version.</summary>
    public static Version? Parse(string? tag)
    {
        var normalized = Normalize(tag);
        return normalized is null ? null : (Version.TryParse(normalized, out var version) ? version : null);
    }

    /// <summary>Normalizes a release tag to a three part version string, or null when there is none.</summary>
    public static string? Normalize(string? tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        var trimmed = tag.Trim();

        if (trimmed.StartsWith('v') || trimmed.StartsWith('V'))
        {
            trimmed = trimmed[1..];
        }

        // "1.2.0-beta.1" and "1.2.0+build5" both mean 1.2.0 for comparison purposes.
        var cut = trimmed.IndexOfAny(['-', '+']);
        if (cut > 0)
        {
            trimmed = trimmed[..cut];
        }

        trimmed = trimmed.Trim();

        if (trimmed.Length == 0 || trimmed.Any(character => character is not (>= '0' and <= '9') and not '.'))
        {
            return null;
        }

        var parts = trimmed.Split('.');
        if (parts.Length > 4)
        {
            return null;
        }

        return parts.Length switch
        {
            1 => trimmed + ".0.0",
            2 => trimmed + ".0",
            _ => trimmed,
        };
    }
}
