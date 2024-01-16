using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// Error status for a meter test system.
/// </summary>
public class ErrorConditions
{
    /// <summary>
    /// Some amplifier in error.
    /// </summary>
    [NotNull, Required]
    public bool HasAmplifierError { get; set; }

    /// <summary>
    /// Some fuse in error.
    /// </summary>
    [NotNull, Required]
    public bool HasFuseError { get; set; }

    /// <summary>
    /// Short circuit on current or voltage.
    /// </summary>
    [NotNull, Required]
    public bool VoltageCurrentIsShort { get; set; }

    /// <summary>
    /// Emergency stop.
    /// </summary>
    [NotNull, Required]
    public bool EmergencyStop { get; set; }

    /// <summary>
    /// Isolation failure.
    /// </summary>
    [NotNull, Required]
    public bool IsolationFailure { get; set; }

    /// <summary>
    /// LWL ident corruped.
    /// </summary>
    [NotNull, Required]
    public bool LwlIdentCorrupted { get; set; }

    /// <summary>
    /// Reference meter data transmission error.
    /// </summary>
    [NotNull, Required]
    public bool ReferenceMeterDataTransmissionError { get; set; }

    /// <summary>
    /// ICT failure.
    /// </summary>
    [NotNull, Required]
    public bool? IctFailure { get; set; }

    /// <summary>
    /// Wrong range reference meter.
    /// </summary>
    [NotNull, Required]
    public bool? WrongRangeReferenceMeter { get; set; }

    /// <summary>
    /// Individual error conditions for all amplifiers.
    /// </summary>
    [NotNull, Required]
    public Dictionary<Amplifiers, AmplifierErrorConditions> Amplifiers { get; set; } = new() {
        { Models.Amplifiers.Current1, new() },
        { Models.Amplifiers.Current2, new() },
        { Models.Amplifiers.Current3, new() },
        { Models.Amplifiers.Voltage1, new() },
        { Models.Amplifiers.Voltage2, new() },
        { Models.Amplifiers.Voltage3, new() },
        { Models.Amplifiers.Auxiliary1, new() },
        { Models.Amplifiers.Auxiliary2, new() },
    };

    /// <summary>
    /// Set if there is any error condition active.
    /// </summary>
    [NotNull, Required]
    public bool HasAnyError =>
        EmergencyStop ||
        HasAmplifierError ||
        HasFuseError ||
        IctFailure == true ||
        IsolationFailure ||
        LwlIdentCorrupted ||
        ReferenceMeterDataTransmissionError ||
        VoltageCurrentIsShort ||
        WrongRangeReferenceMeter == true ||
        Amplifiers.Values.Any(a => a.HasAnyError);
}
