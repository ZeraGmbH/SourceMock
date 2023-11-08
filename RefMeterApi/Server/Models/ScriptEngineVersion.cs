using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RefMeterApi.Models;

/// <summary>
/// Version information of the script engine.
/// </summary>
public class ScriptEngineVersion
{
    /// <summary>
    /// Current version of the engine.
    /// </summary>
    [Required]
    [NotNull]
    public string Version { get; set; } = "0.0";
}
