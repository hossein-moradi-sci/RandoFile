using RandoFile.Core.Abstractions;

namespace RandoFile.Core.Services;

/// <summary>Uniform Fisher-Yates shuffle. Never returns the input collection itself.</summary>
public sealed class FileShuffler : IFileShuffler
{
    private readonly Random _random;

    public FileShuffler()
        : this(Random.Shared)
    {
    }

    /// <summary>Creates a shuffler with a fixed seed, which makes results reproducible in tests.</summary>
    public FileShuffler(int seed)
        : this(new Random(seed))
    {
    }

    public FileShuffler(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var buffer = items.ToArray();
        for (var i = buffer.Length - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }

        return buffer;
    }
}
