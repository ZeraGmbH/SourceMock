using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a single probing operation.
/// </summary>
[JsonDerivedType(typeof(HttpProbe), typeDiscriminator: "HTTP")]
[JsonDerivedType(typeof(IPProbe), typeDiscriminator: "TCPIP")]
[JsonDerivedType(typeof(SerialProbe), typeDiscriminator: "SerialPort")]
[JsonDerivedType(typeof(SerialProbeOverTcp), typeDiscriminator: "SerialPortOverTcp")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "_type")]
public abstract class Probe
{
    /// <summary>
    /// Set the result of the probing.
    /// </summary>
    [NotNull, Required]
    public ProbeResult Result { get; set; }
}