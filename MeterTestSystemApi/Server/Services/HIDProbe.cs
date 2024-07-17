namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a HID probing.
/// </summary>
internal class HIDProbe : Probe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    public required HIDProbeProtocols Protocol { get; set; }

    /// <summary>
    /// Device to use.
    /// </summary>
    public required uint Index { get; set; }

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{DevicePath}: {Protocol}";

    /// <summary>
    /// Get the device path of the HID device.
    /// </summary>
    public string? DevicePath => $"/dev/input/event{Index}";


}

