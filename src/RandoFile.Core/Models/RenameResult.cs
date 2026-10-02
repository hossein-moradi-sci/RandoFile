namespace RandoFile.Core.Models;

/// <summary>A single failure, always linked to the file that caused it.</summary>
public sealed record RenameError(string FileName, string Message);

/// <summary>Outcome of an <c>Execute</c> call.</summary>
public sealed class RenameResult
{
    public required bool Success { get; init; }

    /// <summary>Files the plan wanted to rename.</summary>
    public int Requested { get; init; }

    /// <summary>Files whose name was successfully changed.</summary>
    public int Completed { get; init; }

    /// <summary>True when a failure forced the executor to restore the original names.</summary>
    public bool RolledBack { get; init; }

    /// <summary>True when the operation was stopped by the caller.</summary>
    public bool Cancelled { get; init; }

    public IReadOnlyList<RenameError> Errors { get; init; } = Array.Empty<RenameError>();

    public TimeSpan Duration { get; init; }
}
