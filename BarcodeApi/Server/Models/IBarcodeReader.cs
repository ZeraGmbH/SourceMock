namespace BarcodeApi.Models;

/// <summary>
/// Interface provided by a barcode reader.
/// </summary>
public interface IBarcodeReader
{
    /// <summary>
    /// Will be fired as soon as a barcode is detected.
    /// </summary>
    event Action<string> BarcodeReceived;
}
