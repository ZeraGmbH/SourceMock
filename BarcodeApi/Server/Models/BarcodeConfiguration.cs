namespace BarcodeApi.Models;

/// <summary>
/// Configuration for the barcode reader.
/// </summary>
public class BarcodeConfiguration
{
    /// <summary>
    /// Path to the HID device.
    /// </summary>
    public string? DevicePath { get; set; }
}