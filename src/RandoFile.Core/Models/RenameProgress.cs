namespace RandoFile.Core.Models;

/// <summary>Which half of the two-phase rename is currently running.</summary>
public enum RenameStage
{
    /// <summary>Files are moved to temporary names to free up every original name.</summary>
    Staging = 0,

    /// <summary>Temporary files are moved to their final names.</summary>
    Committing = 1,

    /// <summary>A failure triggered an automatic rollback back to the original names.</summary>
    RollingBack = 2,
}

/// <summary>Progress information reported by the executor.</summary>
/// <param name="Stage">Current phase of the operation.</param>
/// <param name="Processed">Items finished in the current phase.</param>
/// <param name="Total">Items in the current phase.</param>
/// <param name="CurrentFile">File currently being processed, when known.</param>
public sealed record RenameProgress(RenameStage Stage, int Processed, int Total, string? CurrentFile = null)
{
    /// <summary>Progress expressed across both phases, so the bar never jumps backwards.</summary>
    public double OverallPercent
    {
        get
        {
            if (Total <= 0)
            {
                return Stage == RenameStage.Committing ? 100 : 0;
            }

            var phaseOffset = Stage switch
            {
                RenameStage.Committing => 50.0,
                RenameStage.RollingBack => 0.0,
                _ => 0.0,
            };

            return Math.Min(100.0, phaseOffset + Processed * 50.0 / Total);
        }
    }
}
