using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// All possible error conditions of an amplifier.
/// </summary>
public class AmplifierErrorConditions
{
    /// <summary>
    /// General error condition.
    /// </summary>
    [NotNull, Required]
    public bool HasError { get; set; }

    /// <summary>
    /// Short circuit or open line.
    /// </summary>
    [NotNull, Required]
    public bool ShortOrOpen { get; set; }

    /// <summary>
    /// Temperature exceeds limit.
    /// </summary>
    [NotNull, Required]
    public bool Temperature { get; set; }

    /// <summary>
    /// Power supply exceeds lower level.
    /// </summary>
    [NotNull, Required]
    public bool PowerSupply { get; set; }

    /// <summary>
    /// Overload.
    /// </summary>
    [NotNull, Required]
    public bool Overload { get; set; }

    /// <summary>
    /// Group error.
    /// </summary>
    [NotNull, Required]
    public bool GroupError { get; set; }

    /// <summary>
    /// Connection missing (feedback signal).
    /// </summary>
    [NotNull, Required]
    public bool ConnectionMissing { get; set; }

    /// <summary>
    /// Data transmission error to amplifier.
    /// </summary>
    [NotNull, Required]
    public bool DataTransmission { get; set; }

    /// <summary>
    /// Undefined amplifier error.
    /// </summary>
    [NotNull, Required]
    public bool UndefinedError { get; set; }

    /// <summary>
    /// Set if there is any error consition active.
    /// </summary>
    [NotNull, Required]
    public bool HasAnyError =>
       ConnectionMissing ||
       DataTransmission ||
       GroupError ||
       HasError ||
       Overload ||
       PowerSupply ||
       ShortOrOpen ||
       Temperature ||
       UndefinedError;

    /// <inheritdoc/>
    public override string ToString()
    {
        var all = new List<string>();

        if (ConnectionMissing) all.Add("ConnectionMissing");
        if (DataTransmission) all.Add("DataTransmission");
        if (GroupError) all.Add("GroupError");
        if (Overload) all.Add("Overload");
        if (PowerSupply) all.Add("PowerSupply");
        if (ShortOrOpen) all.Add("ShortOrOpen");
        if (Temperature) all.Add("Temperature");
        if (UndefinedError) all.Add("UndefinedError");

        return all.Count > 0 ? string.Join("|", all) : HasError ? "error" : "ok";
    }
}
