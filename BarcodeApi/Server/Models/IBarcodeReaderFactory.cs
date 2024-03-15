namespace BarcodeApi.Models;

/// <summary>
/// Factory interface to create barcode readers.
/// </summary>
public interface IBarcodeReaderFactory
{
    /// <summary>
    /// Create a new bar code reader based on the given configuration.
    /// </summary>
    /// <param name="configuration">Configuration to use.</param>
    public void Initialize(BarcodeConfiguration? configuration);

    /// <summary>
    /// Get the barcode reader to use - will be available after initialisation.
    /// </summary>
    public IBarcodeReader BarcodeReader { get; }
}
