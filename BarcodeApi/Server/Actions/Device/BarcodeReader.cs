using BarcodeApi.Models;

namespace BarcodeApi.Actions.Device;

/// <summary>
/// Access a barcode reader using a file path.
/// </summary>
public class BarcodeReader : IBarcodeReader
{
    /// <inheritdoc/>
    public event Action<string> BarcodeReceived = null!;

    /// <summary>
    /// Simulate a barcode.
    /// </summary>
    /// <param name="code">Barcode to send.</param>
    public void Fire(string code) => BarcodeReceived?.Invoke(code);
}
