using BarcodeApi.Models;

namespace BarcodeApi.Actions.Device;

/// <summary>
/// Simulator for a barcode reader.
/// </summary>
public class BarcodeReaderMock : IBarcodeReader
{
    /// <inheritdoc/>
    public event Action<string> BarcodeReceived = null!;

    /// <summary>
    /// Simulate a barcode.
    /// </summary>
    /// <param name="code">Barcode to send.</param>
    public void Fire(string code) => BarcodeReceived?.Invoke(code);
}
