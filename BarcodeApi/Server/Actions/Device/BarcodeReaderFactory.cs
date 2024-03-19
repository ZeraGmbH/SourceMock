using BarcodeApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BarcodeApi.Actions.Device;

/// <summary>
/// Factory to create barcode readers from a configuration.
/// </summary>
/// <param name="services">Dependency injection.</param>
/// <param name="logger">Logging helper.</param>
public class BarcodeReaderFactory(IServiceProvider services, ILogger<BarcodeReaderFactory> logger) : IBarcodeReaderFactory, IDisposable
{
    private readonly object _sync = new();

    private bool _initialized = false;

    private IBarcodeReader? _reader;

    /// <inheritdoc/>
    public IBarcodeReader BarcodeReader
    {
        get
        {
            /* Wait until instance has been created. */
            lock (_sync)
                while (!_initialized)
                    Monitor.Wait(_sync);

            return _reader!;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
            _initialized = true;
    }

    /// <inheritdoc/>
    public void Initialize(BarcodeConfiguration? configuration)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("Barcode reader already initialized");

            try
            {
                if (!string.IsNullOrEmpty(configuration?.DevicePath))
                    _reader = new BarcodeReader(configuration.DevicePath, services.GetRequiredService<ILogger<BarcodeReader>>());
            }
            catch (Exception e)
            {
                logger.LogCritical("Unable to create barcode reader: {Exception}", e.Message);
            }
            finally
            {
                /* Use fallback so that there is ALWAYS a barcode reader. */
                _reader ??= services.GetRequiredService<BarcodeReaderMock>();

                /* Use the new instance. */
                _initialized = true;

                /* Signal availability of barcode reader. */
                Monitor.PulseAll(_sync);
            }
        }
    }
}
