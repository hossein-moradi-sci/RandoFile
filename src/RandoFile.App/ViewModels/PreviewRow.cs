namespace RandoFile.App.ViewModels;

/// <summary>One line of the preview list: the current name and the name it will get.</summary>
public sealed class PreviewRow
{
    public PreviewRow(string originalName, string newName, bool isChanged)
    {
        OriginalName = originalName;
        NewName = newName;
        IsChanged = isChanged;
    }

    public string OriginalName { get; }

    public string NewName { get; }

    /// <summary>False when the shuffle kept the file's current name.</summary>
    public bool IsChanged { get; }
}
