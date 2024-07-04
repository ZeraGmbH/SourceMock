using System.Xml.Serialization;
using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

/// <summary>
/// Describes a specific loadpoint.
/// </summary>
/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] Phases
///     [1] VoltageNeutralConnected
///     [2] Frequency
/// </remarks>
[Serializable]
public class TargetLoadpointNGX : AbstractLoadpoint<TargetLoadpointPhaseNGX, ActivatableElectricalQuantity<Voltage>, ActivatableElectricalQuantity<Current>>
{
    /// <summary>
    /// Determines whether or not the voltages neutral conductor is to be connected.
    /// </summary>
    public bool VoltageNeutralConnected { get; set; }

    /// <summary>
    /// The frequency.
    /// </summary>
    public Frequency Frequency { get; set; } = new();

    /// <summary>
    /// Report the report name of this loadpoint.
    /// </summary>
    [XmlIgnore]
    public string? ReportName
    {
        get
        {
            /* No loadpoint at all. */
            var phases = Phases ?? [];

            if (phases.Count < 1) return null;

            /* Check first active phase - prefer the case wehere voltage and current are on..*/
            var phase =
                phases.FirstOrDefault(p => p.Voltage?.On == true && p.Current?.On == true) ??
                phases.FirstOrDefault(p => p.Voltage?.On == true || p.Current?.On == true);

            if (phase == null) return null;

            /* Get components. */
            var vComponent = phase.Voltage;
            var cComponent = phase.Current;

            /* Get quantities. */
            var voltage = vComponent?.On == true ? ((vComponent?.AcComponent?.Rms ?? new()) + (vComponent?.DcComponent ?? new())) : new();
            var current = cComponent?.On == true ? ((cComponent?.AcComponent?.Rms ?? new()) + (cComponent?.DcComponent ?? new())) : new();

            /* See if there is anything on the wire. */
            if ((double)voltage == 0 && (double)current == 0) return null;

            /* Calculate the power factor. */
            var powerFactor = string.Empty;

            if (vComponent?.On == true && vComponent.AcComponent != null)
                if (cComponent?.On == true && cComponent.AcComponent != null)
                {
                    /* Use arcsin to differentiate between inductive and capacitive. */
                    var pfRaw = (vComponent.AcComponent.Angle - cComponent.AcComponent.Angle).Sin();
                    var ind = pfRaw >= 0 ? "ind" : "cap";

                    /* Calculate cos using Pythagoras. */
                    var pf = Math.Sqrt(1 - pfRaw * pfRaw);

                    powerFactor = $"{Math.Abs(pf):G3} {ind}";
                }

            /* Construct name. */
            return string.IsNullOrEmpty(powerFactor) ? $"({voltage}V/{current}A)" : $"({voltage}V/{current}A/{powerFactor})";
        }
    }
}
