namespace RandoFile.Core.Abstractions;

/// <summary>Shuffles a list of items. Abstracted so tests can use a deterministic implementation.</summary>
public interface IFileShuffler
{
    IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items);
}

/// <summary>Lists the candidate files of a folder.</summary>
public interface IFileScanner
{
    IReadOnlyList<string> Scan(string folder, bool includeSubfolders = false);
}
