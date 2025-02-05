using ErrorCalculatorApi.Actions;
using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Actions;
using ZERA.WebSam.Shared.Provider;
using System.Text.RegularExpressions;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.MeterTestSystem;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZeraDevices.Source.FG30x;
using ZeraDevices.Source.MT768;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of a meter test system implementation using the
/// serial port protocol against a frequency generator.
/// </summary>
public class SerialPortFGMeterTestSystem : IMeterTestSystem, ISerialPortOwner
{
    /// <summary>
    /// Pattern for error conditions.
    /// </summary>
    private static readonly Regex _smRegEx = new("^SM([0-9A-Fa-f]+)$");

    /// <summary>
    /// Physical connection to the frequency generator.
    /// </summary>
    private readonly ISerialPortConnectionExecutor _device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortFGMeterTestSystem> _logger;

    /// <summary>
    /// Dependency injection system to allow dynamic switching of source, 
    /// reference meter and error calclulator implemtations when the physical
    /// configuration changes.
    /// </summary>
    private readonly IServiceProvider _services;

    /// <inheritdoc/>
    public bool HasSource { get; private set; } = true;

    /// <inheritdoc/>
    public bool HasDosage { get; private set; } = true;

    /// <inheritdoc/>
    private AmplifiersAndReferenceMeter _amplifiersAndReferenceMeter = null!;

    /// <inheritdoc/>
    public Task<AmplifiersAndReferenceMeter> GetAmplifiersAndReferenceMeterAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_amplifiersAndReferenceMeter);

    /// <inheritdoc/>
    public ISource Source { get; private set; } = new UnavailableSource();

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; private set; } = new UnavailableReferenceMeter();

    private List<IErrorCalculator> _errorCalculators = [new UnavailableErrorCalculator()];

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => _errorCalculators.ToArray();

    /// <inheritdoc/>
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    /// <param name="services">Dependency injection system.</param>
    public SerialPortFGMeterTestSystem([FromKeyedServices("MeterTestSystem")] ISerialPortConnection device, ILogger<SerialPortFGMeterTestSystem> logger, IServiceProvider services)
    {
        _device = device.CreateExecutor(InterfaceLogSourceTypes.MeterTestSystem);
        _logger = logger;
        _services = services;

        /* Register out-of-band processing of error conditions. */
        device.RegisterEvent(_smRegEx, reply => ErrorConditionsChanged?.Invoke(ErrorConditionParser.Parse(reply.Groups[1].Value, true)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task InitializeFGAsync(IInterfaceLogger logger)
    {
        _logger.LogInformation("Starting up FG30x hardware");

        await _device.ExecuteAsync(logger, SerialPortRequest.Create("RE1", "OKRE"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("SE1", "OKSE"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("HP11", "OKHP"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("OL1111111111111111", "OKOL"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("SF0", "OKSF"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("HEAAAAAAAAAAAAAAAA", "OKHE"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"3CM4", "OK3CM4"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"2S1xx", "2OK"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"2X0000", "2OK"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"2X0200", "2OK"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create("OF11111111+000.000", "OKOF"))[0];
    }

    /// <summary>
    /// Called before the system shuts down.
    /// </summary>
    /// <param name="logger">Logging helper.</param>
    public async Task DeinitializeFGAsync(IInterfaceLogger logger)
    {
        _logger.LogInformation("Shutting down FG30x hardware");

        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"2S0xx", "2OK"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"OL1111111111111111", "OKOL"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"PL", "OKPL"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"SF0", "OKSF"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"UPAER000.000000.00S000.000120.00T000.000240.00", "OKUP"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"UIAAAAAAAAA", "OKUI"))[0];
        await _device.ExecuteAsync(logger, SerialPortRequest.Create($"HP00", "OKHP"))[0];
    }

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) =>
        Task.FromResult(new MeterTestSystemCapabilities
        {
            SupportedCurrentAmplifiers = {
                CurrentAmplifiers.SCG1020,
                CurrentAmplifiers.VI201x0,
                CurrentAmplifiers.VI201x0x1,
                CurrentAmplifiers.VI201x1,
                CurrentAmplifiers.VI202x0,
                CurrentAmplifiers.VI202x0x1,
                CurrentAmplifiers.VI202x0x2,
                CurrentAmplifiers.VI202x0x5,
                CurrentAmplifiers.VI220,
                CurrentAmplifiers.VI221x0,
                CurrentAmplifiers.VI222x0,
                CurrentAmplifiers.VI222x0x1,
                CurrentAmplifiers.VUI301,
                CurrentAmplifiers.VUI302,
            },
            SupportedCurrentAuxiliaries = {
                CurrentAuxiliaries.V200,
                CurrentAuxiliaries.VI201x0,
                CurrentAuxiliaries.VI201x0x1,
                CurrentAuxiliaries.VI201x1,
                CurrentAuxiliaries.VI202x0,
                CurrentAuxiliaries.VI221x0,
                CurrentAuxiliaries.VI222x0,
                CurrentAuxiliaries.VUI301,
                CurrentAuxiliaries.VUI302,
            },
            SupportedReferenceMeters = {
                ReferenceMeters.COM3003,
                ReferenceMeters.COM3003x1x2,
                ReferenceMeters.COM5003x0x1,
                ReferenceMeters.COM5003x1,
                ReferenceMeters.EPZ103,
                ReferenceMeters.EPZ303x1,
                ReferenceMeters.EPZ303x10,
                ReferenceMeters.EPZ303x10x1,
                ReferenceMeters.EPZ303x5,
                ReferenceMeters.EPZ303x8,
                ReferenceMeters.EPZ303x8x1,
                ReferenceMeters.EPZ303x9
            },
            SupportedVoltageAmplifiers = {
                VoltageAmplifiers.SVG3020,
                VoltageAmplifiers.VU211x0,
                VoltageAmplifiers.VU211x1,
                VoltageAmplifiers.VU211x2,
                VoltageAmplifiers.VU220,
                VoltageAmplifiers.VU220x01,
                VoltageAmplifiers.VU220x02,
                VoltageAmplifiers.VU220x03,
                VoltageAmplifiers.VU220x04,
                VoltageAmplifiers.VU221x0,
                VoltageAmplifiers.VU221x0x2,
                VoltageAmplifiers.VU221x0x3,
                VoltageAmplifiers.VU221x1,
                VoltageAmplifiers.VU221x2,
                VoltageAmplifiers.VU221x3,
                VoltageAmplifiers.VUI301,
                VoltageAmplifiers.VUI302,
            },
            SupportedVoltageAuxiliaries = {
                VoltageAuxiliaries.V210,
                VoltageAuxiliaries.VU211x0,
                VoltageAuxiliaries.VU211x1,
                VoltageAuxiliaries.VU211x2,
                VoltageAuxiliaries.VU221x0,
                VoltageAuxiliaries.VU221x1,
                VoltageAuxiliaries.VU221x2,
                VoltageAuxiliaries.VU221x3,
                VoltageAuxiliaries.VUI301,
                VoltageAuxiliaries.VUI302,
            }
        });

    /// <inheritdoc/>
    public async Task SetAmplifiersAndReferenceMeterAsync(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings)
    {
        /* Validate all input parameters against our own capabilities. */
        var capabilities = await GetCapabilitiesAsync(logger);

        if (!capabilities.SupportedVoltageAmplifiers.Contains(settings.VoltageAmplifier))
        {
            _logger.LogDebug($"Unsupported voltage amplifier {settings.VoltageAmplifier}");

            throw new ArgumentException("voltage");
        }

        if (!capabilities.SupportedVoltageAuxiliaries.Contains(settings.VoltageAuxiliary))
        {
            _logger.LogDebug($"Unsupported auxiliary voltage amplifier {settings.VoltageAuxiliary}");

            throw new ArgumentException("voltageAux");
        }

        if (!capabilities.SupportedCurrentAmplifiers.Contains(settings.CurrentAmplifier))
        {
            _logger.LogDebug($"Unsupported current amplifier {settings.CurrentAmplifier}");

            throw new ArgumentException("current");
        }

        if (!capabilities.SupportedCurrentAuxiliaries.Contains(settings.CurrentAuxiliary))
        {
            _logger.LogDebug($"Unsupported auxiliary current amplifier {settings.CurrentAuxiliary}");

            throw new ArgumentException("currentAux");
        }

        if (!capabilities.SupportedReferenceMeters.Contains(settings.ReferenceMeter))
        {
            _logger.LogDebug($"Unsupported reference meter");

            throw new ArgumentException("referenceMeter");
        }

        /* Create new instances of all connected sub devices. */
        var refMeter = _services.GetRequiredService<ISerialPortFGRefMeter>();
        var source = _services.GetRequiredService<ISerialPortFGSource>();

        /* Configure the sub devices if necessary. */
        refMeter.SetReferenceMeter(settings.ReferenceMeter);

        await source.SetAmplifiersAsync(logger, settings.VoltageAmplifier, settings.CurrentAmplifier, settings.VoltageAuxiliary, settings.CurrentAuxiliary);

        try
        {
            /* Get all API codes for amplifiers and reference meter - to make source code a bit more readable. */
            var auxCurrentCode = CodeMappings.AuxCurrent[settings.CurrentAuxiliary];
            var auxVoltageCode = CodeMappings.AuxVoltage[settings.VoltageAuxiliary];
            var currentCode = CodeMappings.Current[settings.CurrentAmplifier];
            var refMeterCode = CodeMappings.RefMeter[settings.ReferenceMeter];
            var voltageCode = CodeMappings.Voltage[settings.VoltageAmplifier];

            /* Send the combined command to the meter test system. */
            await _device.ExecuteAsync(logger, SerialPortRequest.Create($"ZP{voltageCode:00}{currentCode:00}{auxVoltageCode:00}{auxCurrentCode:00}{refMeterCode:00}", "OKZP"))[0];
        }
        catch (Exception)
        {
            /* Do not update the current configuration since the frequency generator rejected the new settings. */
            source = null;

            throw;
        }
        finally
        {
            if (source != null)
            {
                /* Update the implementation references if the frequency generator accepted the new configuration. */
                _amplifiersAndReferenceMeter = settings;
                RefMeter = refMeter;
                Source = source;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<MeterTestSystemFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger)
    {
        /* Send command and check reply. */
        var request = SerialPortRequest.Create("TS", new Regex("^TS(.{8})(.{4})$"));

        await _device.ExecuteAsync(logger, request)[0];

        /* Create response structure. */
        return new()
        {
            ModelName = request.EndMatch!.Groups[1].Value.Trim(),
            Version = request.EndMatch!.Groups[2].Value.Trim()
        };
    }

    /// <inheritdoc/>
    public async Task<ErrorConditions> GetErrorConditionsAsync(IInterfaceLogger logger)
    {
        /* Send command and check reply. */
        var request = SerialPortRequest.Create("SM", _smRegEx);

        await _device.ExecuteAsync(logger, request)[0];

        /* Create response structure. */
        return ErrorConditionParser.Parse(request.EndMatch!.Groups[1].Value, true);
    }

    /// <summary>
    /// Enable error condition generation.
    /// </summary>
    /// <param name="logger">Logger to use.</param>
    public Task ActivateErrorConditionsAsync(IInterfaceLogger logger) => _device.ExecuteAsync(logger, SerialPortRequest.Create("SE1", "OKSE"))[0];

    /// <summary>
    /// Configure the error calculators.
    /// </summary>
    /// <param name="config">List of error calculators to use.</param>
    /// <param name="factory">Factory to create error calculators.</param>
    public async Task ConfigureErrorCalculatorsAsync(List<ErrorCalculatorConfiguration> config, IErrorCalculatorFactory factory)
    {
        if (config.Count < 1) return;

        /* Error calculators. */
        var errorCalculators = new List<IErrorCalculatorInternal>();

        try
        {
            /* Create calculators based on configuration. */
            for (var i = 0; i < config.Count; i++)
                errorCalculators.Add(await factory.CreateAsync(i, config[i]));
        }
        catch (Exception)
        {
            /* Release anything we have configured so far. */
            errorCalculators.ForEach(ec => ec.Destroy());

            throw;
        }

        /* Use. */
        _errorCalculators.Clear();
        _errorCalculators.AddRange(errorCalculators);
    }

    /// <summary>
    /// See if we want to use another reference meter - only sources are controlled
    /// using the FG.
    /// </summary>
    /// <param name="configuration">Optinal configuration.</param>
    public IDisposable? ConfigureExternalReferenceMeterAsync(ExternalReferenceMeterConfiguration? configuration)
    {
        switch (configuration?.Type)
        {
            case ExternalReferenceMeterTypes.MT3000:
                {
                    var config = configuration.SerialPort;

                    if (config == null) break;

                    var portLogger = _services.GetRequiredService<ILogger<SerialPortConnection>>();

                    var connection = config.ConfigurationType switch
                    {
                        SerialPortConfigurationTypes.Device => SerialPortConnection.FromSerialPort(config.Endpoint!, config.SerialPortOptions, portLogger),
                        SerialPortConfigurationTypes.Network => SerialPortConnection.FromNetwork(config.Endpoint!, portLogger),
                        SerialPortConfigurationTypes.Mock => SerialPortConnection.FromMock<SerialPortMTMock>(portLogger),
                        _ => throw new NotSupportedException($"Unknown serial port configuration type {config.ConfigurationType}"),
                    };

                    try
                    {
                        RefMeter = new SerialPortMTRefMeter(connection, _services.GetRequiredService<ILogger<SerialPortMTRefMeter>>());
                    }
                    catch
                    {
                        using (connection)
                            throw;
                    }

                    return connection;
                }
            case null:
                break;
            default:
                throw new ArgumentException($"unsupported external reference meter type '{configuration?.Type}'", nameof(configuration));
        }

        return null;
    }

    /// <summary>
    /// Disable source.
    /// </summary>
    public void NoSource()
    {
        Source = new UnavailableSource();

        HasDosage = false;
        HasSource = false;
    }

    /// <inheritdoc/>
    public async Task<SerialPortReply[]> ExecuteAsync(IEnumerable<SerialPortCommand> requests, IInterfaceLogger logger)
    {
        var commands =
            requests
                .Select(r =>
                    r.UseRegularExpression
                        ? SerialPortRequest.Create(r.Command, new Regex(r.Reply))
                        : SerialPortRequest.Create(r.Command, r.Reply))
                .ToArray();

        await Task.WhenAll(_device.ExecuteAsync(logger, commands));

        return [.. commands.Select(SerialPortReply.FromRequest)];
    }
}
