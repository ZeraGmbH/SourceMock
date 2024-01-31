using System.Diagnostics;
using BarcodeApi.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace BarcodeApi.Actions.Device;

/// <summary>
/// Access a barcode reader using a file path.
/// </summary>
public class BarcodeReader : IBarcodeReader, IDisposable
{
    /// <inheritdoc/>
    public event Action<string> BarcodeReceived = null!;

    private readonly ILogger<BarcodeReader> _logger;

    private readonly Stream _device = null!;

    /// <summary>
    /// Initialize a new barcode reader instance.
    /// </summary>
    /// <param name="device">Path to the device to use.</param>
    /// <param name="logger">Logging helper.</param>
    public BarcodeReader(string device, ILogger<BarcodeReader> logger)
    {
        _logger = logger;

        try
        {
            _device = new FileStream(device, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Unable to access barcode reader: {Exception}", e.Message);
        }
    }

    /// <summary>
    /// Simulate a barcode.
    /// </summary>
    /// <param name="code">Barcode to send.</param>
    public void Fire(string code) => BarcodeReceived?.Invoke(code);

    /// <summary>
    /// Close connection to the device.
    /// </summary>
    public void Dispose() => _device?.Dispose();
}
