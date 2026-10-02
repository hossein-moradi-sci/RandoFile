using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RandoFile.App.ViewModels;

namespace RandoFile.Core.Tests;

/// <summary>
/// Guards the XAML against the binding mistake that only shows up at runtime: several WPF properties
/// (<c>ProgressBar.Value</c>, <c>TextBox.Text</c>, <c>ToggleButton.IsChecked</c>, …) bind
/// <see cref="System.Windows.Data.BindingMode.TwoWay"/> by default, so pointing them at a computed,
/// read-only view model property throws the moment the window is first laid out.
/// </summary>
public class XamlBindingTests
{
    /// <summary>
    /// Target properties whose metadata sets <c>BindsTwoWayByDefault</c>. A binding to one of these
    /// must resolve to a property with a public setter unless the markup asks for <c>Mode=OneWay</c>.
    /// </summary>
    private static readonly Dictionary<(string Element, string Property), string> TwoWayByDefault = new()
    {
        [("ProgressBar", "Value")] = "RangeBase.Value",
        [("Slider", "Value")] = "RangeBase.Value",
        [("TextBox", "Text")] = "TextBoxBase.Text",
        [("CheckBox", "IsChecked")] = "ToggleButton.IsChecked",
        [("RadioButton", "IsChecked")] = "ToggleButton.IsChecked",
        [("ComboBox", "SelectedItem")] = "Selector.SelectedItem",
        [("ComboBox", "SelectedValue")] = "Selector.SelectedValue",
        [("ListBox", "SelectedItem")] = "Selector.SelectedItem",
        [("ListView", "SelectedItem")] = "Selector.SelectedItem",
        [("TabControl", "SelectedItem")] = "Selector.SelectedItem",
        [("Expander", "IsExpanded")] = "Expander.IsExpanded",
    };

    /// <summary>Every type a <c>{Binding}</c> in the window markup can point at.</summary>
    private static readonly Type[] BindingSources =
    [
        typeof(MainViewModel),
        typeof(PreviewRow),
        typeof(SettingsOption),
    ];

    [Fact]
    public void Two_way_bindings_never_target_a_read_only_property()
    {
        var problems = new List<string>();

        foreach (var (file, document) in LoadViews())
        {
            Inspect(document.Root!, file, problems);
        }

        Assert.True(
            problems.Count == 0,
            "Two-way bindings must target a writable property:" + Environment.NewLine + string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void Every_binding_path_points_at_a_real_property()
    {
        var problems = new List<string>();

        foreach (var (file, document) in LoadViews())
        {
            foreach (var element in document.Descendants())
            {
                foreach (var attribute in element.Attributes())
                {
                    var path = TryReadPath(attribute.Value, out var mode);
                    if (path is null || mode == "OneWayToSource" || mode == "OneTime")
                    {
                        continue;
                    }

                    var name = path.Split('.')[0];
                    if (name.Length == 0)
                    {
                        continue;
                    }

                    // Relative sources and item bindings address something we cannot see statically.
                    if (attribute.Value.Contains("RelativeSource", StringComparison.Ordinal) ||
                        attribute.Value.Contains("AncestorType", StringComparison.Ordinal) ||
                        attribute.Value.Contains("ElementName", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!BindingSources.Any(type => type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) is not null))
                    {
                        problems.Add($"{file}: <{element.Name.LocalName} {attribute.Name}> binds to unknown '{path}'");
                    }
                }
            }
        }

        Assert.True(
            problems.Count == 0,
            "Unknown binding paths:" + Environment.NewLine + string.Join(Environment.NewLine, problems));
    }

    private static void Inspect(XElement root, string file, List<string> problems)
    {
        foreach (var element in root.DescendantsAndSelf())
        {
            var name = element.Name.LocalName;

            // A Setter inherits the TargetType of the Style that owns it.
            var targetType = name == "Setter"
                ? element.Ancestors("Style").FirstOrDefault()?.Attribute("TargetType")?.Value
                : name;

            foreach (var attribute in element.Attributes())
            {
                if (targetType is null || !TwoWayByDefault.TryGetValue((targetType, attribute.Name.LocalName), out var dependencyProperty))
                {
                    continue;
                }

                if (TryReadPath(attribute.Value, out var mode) is not { } path)
                {
                    continue;
                }

                if (mode is "OneWay" or "OneTime")
                {
                    continue;
                }

                var propertyName = path.Split('.')[0];
                var matches = BindingSources
                    .Select(type => type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance))
                    .Where(property => property is not null)
                    .Cast<PropertyInfo>()
                    .ToList();

                if (matches.Count == 0)
                {
                    continue;
                }

                var readOnly = matches.Where(property => property.SetMethod is null || !property.SetMethod.IsPublic).ToList();
                if (readOnly.Count == 0)
                {
                    continue;
                }

                problems.Add(
                    $"{file}: <{name} {attribute.Name}> binds {dependencyProperty} two-way to " +
                    $"'{path}' but {string.Join(", ", readOnly.Select(property => property.DeclaringType?.Name))}.{propertyName} is read-only; " +
                    "add Mode=OneWay or a public setter");
            }
        }
    }

    /// <summary>
    /// Returns the bound property path and the explicit <c>Mode</c> of a <c>{Binding ...}</c> value,
    /// or <c>null</c> when the attribute is not a plain binding.
    /// </summary>
    private static string? TryReadPath(string value, out string? mode)
    {
        mode = null;

        const string prefix = "{Binding";
        var start = value.IndexOf(prefix, StringComparison.Ordinal);
        if (start < 0)
        {
            return null;
        }

        var end = value.IndexOf('}', start);
        if (end < 0)
        {
            return null;
        }

        // Nested braces (converters, static resources) mean this is not a simple path binding.
        var expression = value.Substring(start + prefix.Length, end - start - prefix.Length);

        var parts = expression.Split(',');
        var path = parts[0].Trim();

        foreach (var part in parts.Skip(1))
        {
            var option = part.Trim();
            if (option.StartsWith("Mode=", StringComparison.Ordinal))
            {
                mode = option["Mode=".Length..].Trim();
            }
        }

        return path.Length == 0 || path.Contains('(') ? null : path;
    }

    private static IEnumerable<(string File, XDocument Document)> LoadViews()
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "Xaml");
        Assert.True(Directory.Exists(folder), $"Missing markup folder: {folder}");

        return Directory.GetFiles(folder, "*.xaml")
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => (Path.GetFileName(path), XDocument.Load(path)))
            .ToList();
    }
}
