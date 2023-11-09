using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ScriptApi.Models;

/// <summary>
/// 
/// </summary>
public class StartScriptRequest
{
    /// <summary>
    /// 
    /// </summary>
    [Required, NotNull]
    public string Name { get; set; } = null!;
}
