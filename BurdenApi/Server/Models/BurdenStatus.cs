using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Information on the current burden status.
/// </summary>
public class BurdenStatus
{
    /// <summary>
    /// Name of the burden.
    /// </summary>
    [NotNull, Required]
    public string Burden { get; set; } = null!;

    /// <summary>
    /// Current active range.
    /// </summary>
    [NotNull, Required]
    public string Range { get; set; } = null!;

    /// <summary>
    /// Current selected step.
    /// </summary>
    [NotNull, Required]
    public string Step { get; set; } = null!;

    /// <summary>
    /// Set if the burden is active.
    /// </summary>
    [NotNull, Required]
    public bool Active { get; set; }
}