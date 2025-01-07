using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Result of a prepare information.
/// </summary>
public class PrepareResult
{
    /// <summary>
    /// Factor used - may be clipped due to voltage restrictions.
    /// </summary>
    [NotNull, Required]
    public double Factor { get; set; }

    /// <summary>
    /// Primary range choosen.
    /// </summary>
    [NotNull, Required]
    public double Range { get; set; }
}
