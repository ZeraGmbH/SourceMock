using System.Text.RegularExpressions;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace BarcodeApi.Actions;

/// <summary>
/// Information on system devices.
/// </summary>
public class InputDeviceManager(ILogger<InputDeviceManager> logger, InputDeviceManager.IInputDeviceProvider? deviceFile = null) : IInputDeviceManager
{
    /// <summary>
    /// For unit-
    /// </summary>
    public interface IInputDeviceProvider
    {
        /// <summary>
        /// Report the content of the device file.
        /// </summary>
        string DeviceFile { get; }
    }

    private class InputDevice : IInputDevice
    {
        private static readonly Dictionary<char, Action<InputDevice, string>> _Processors = new() {
            { 'I', (d,l) => d.ParseInfo(l) },
            { 'N', (d,l) => d.ParseString(l) },
            { 'P', (d,l) => d.ParseRaw(l) },
            { 'S', (d,l) => d.ParseRaw(l) },
            { 'U', (d,l) => d.ParseRaw(l) },
            { 'H', (d,l) => d.ParseList(l) },
            { 'B', (d,l) => d.ParseBinary(l) },
         };

        private static readonly Regex _LineReg = new($"^([{string.Join("", _Processors.Keys)}]):\\s+(.*)\\s*$");

        private static readonly Regex _RawReg = new("^\\s*([^=]+)=(.*)\\s*$");

        private static readonly Regex _StringReg = new("^\\s*([^=]+)=\"(.*)\"\\s*$");

        private readonly Dictionary<string, string> _Props = [];

        private readonly Dictionary<string, HashSet<string>> _Lists = [];

        private readonly Dictionary<string, List<ulong>> _Binaries = [];

        public void ProcessLine(string line)
        {
            var match = _LineReg.Match(line);

            if (match.Success && _Processors.TryGetValue(match.Groups[1].Value[0], out var processor))
                processor(this, match.Groups[2].Value);
            else
                Console.WriteLine(match.Groups[1].Value[0]);
        }

        private void ParseRaw(string data)
        {
            var match = _RawReg.Match(data);

            if (match.Success)
                _Props.Add(match.Groups[1].Value, match.Groups[2].Value);
        }

        private void ParseInfo(string data)
        {
            foreach (var prop in data.Split(' '))
                ParseRaw(prop);
        }

        private void ParseString(string data)
        {
            var match = _StringReg.Match(data);

            if (match.Success)
                _Props.Add(match.Groups[1].Value, match.Groups[2].Value);
        }

        private void ParseList(string data)
        {
            var match = _RawReg.Match(data);

            if (match.Success)
                _Lists.Add(
                    match.Groups[1].Value,
                    [.. match.Groups[2].Value
                        .Split(' ')
                        .Select(h => h.Trim())
                        .Where(h => !string.IsNullOrEmpty(h))
                    ]);
        }

        private void ParseBinary(string data)
        {
            var match = _RawReg.Match(data);

            if (!match.Success) return;

            var parts = new List<ulong>();

            foreach (var bits in match.Groups[2].Value.Split(' '))
                parts.Insert(0, ulong.Parse(bits.Trim(), System.Globalization.NumberStyles.HexNumber));

            _Binaries.Add(match.Groups[1].Value, parts);
        }

        public string? GetProperty(string key) => _Props.TryGetValue(key, out var value) ? value : null;

        public HashSet<string>? GetList(string key) => _Lists.TryGetValue(key, out var value) ? [.. value] : null;

        public ulong[]? GetBits(string key) => _Binaries.TryGetValue(key, out var value) ? [.. value] : null;
    }

    private readonly object _sync = new();

    private Task<List<IInputDevice>>? _loader;

    /// <inheritdoc/>
    public Task<List<IInputDevice>> GetAsync()
    {
        lock (_sync)
            return _loader ??= LoadAsync();
    }

    private async Task<List<IInputDevice>> LoadAsync()
    {
        var devices = new List<IInputDevice>();

        try
        {
            InputDevice? current = null;

            foreach (var line in deviceFile?.DeviceFile.Split("\n") ?? await File.ReadAllLinesAsync("/proc/bus/input/devices"))
                if (string.IsNullOrEmpty(line))
                    current = null;
                else
                {
                    if (current == null)
                    {
                        current = new();

                        devices.Add(current);
                    }

                    current.ProcessLine(line);
                }
        }
        catch (Exception e)
        {
            logger.LogError("unable to retrieve sytsem input devices: {Exception}", e.Message);
        }

        return devices;
    }
}