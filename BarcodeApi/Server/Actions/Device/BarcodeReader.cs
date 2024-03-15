using System.Runtime.InteropServices;
using System.Text;
using BarcodeApi.Models;
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

    private readonly string _path;

    private FileStream _device = null!;

    private readonly byte[] _buffer = new byte[24];

    private readonly GCHandle _bufPtr;

    private readonly StringBuilder _collector = new();

    private Dictionary<KeyCodes, char> _currentCodeMap = KeyMaps.BarcodeRegular;

    private Thread? _connector;

    /// <summary>
    /// Initialize a new barcode reader instance.
    /// </summary>
    /// <param name="device">Path to the device to use.</param>
    /// <param name="logger">Logging helper.</param>
    public BarcodeReader(string device, ILogger<BarcodeReader> logger)
    {
        _bufPtr = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

        _logger = logger;
        _path = device;

        _connector = new Thread(Reconnect);
        _connector.Start();
    }

    private void Reconnect()
    {
        for (; ; )
            try
            {
                /* Do proper cleanup. */
                Disconnect();

                /* See if we are already disposed. */
                if (!_bufPtr.IsAllocated) return;

                try
                {
                    /* Open device file. */
                    _device = new FileStream(_path, FileMode.Open, FileAccess.Read);

                    /* Start the read-out. */
                    for (; ; ) ReadInputEvent();
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Unable to access barcode reader: {Exception}", e.Message);

                    /* Do proper cleanup. */
                    Disconnect();

                    /* Try again later. */
                    Thread.Sleep(5000);
                }
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
    }

    /// <summary>
    /// Process an incoming event.
    /// </summary>
    private void ReadInputEvent()
    {
        /* Should be a full event. */
        if (_device.Read(_buffer) != _buffer.Length) return;

        /* Only EV_KEY events. */
        var bufPtr = _bufPtr.AddrOfPinnedObject();

        if (Marshal.ReadInt16(bufPtr, 16) != 1) return;

        /* Only key up events. */
        if (Marshal.ReadInt32(bufPtr, 20) != 0) return;

        /* End of bar code detected. */
        var code = (KeyCodes)Marshal.ReadInt16(bufPtr, 18);

        if (code == KeyCodes.KEY_ENTER)
        {
            /* Get the code. */
            var barcode = _collector.ToString();

            /* Reset all. */
            _currentCodeMap = KeyMaps.BarcodeRegular;
            _collector.Clear();

            /* Dispatch code and read next. */
            BarcodeReceived?.Invoke(barcode);

            _logger.LogTrace("Dispatching bar code {Code}", barcode);

            return;
        }

        /* Use SHIFT map. */
        if (code == KeyCodes.KEY_LEFTSHIFT)
            _currentCodeMap = KeyMaps.BarcodeShifted;
        else
        {
            /* Merge all known keys to the bar code. */
            if (_currentCodeMap.TryGetValue(code, out var supported)) _collector.Append(supported);

            /* Always back to the regular map. */
            _currentCodeMap = KeyMaps.BarcodeRegular;
        }
    }

    private void Disconnect()
    {
        _device?.Dispose();
        _device = null!;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Disconnect();

        _bufPtr.Free();

        _connector?.Interrupt();
        _connector = null;
    }
}
