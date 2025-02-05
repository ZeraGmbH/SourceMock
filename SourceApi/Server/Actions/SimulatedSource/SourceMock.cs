using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Dosage;

namespace SourceApi.Actions.Source;

public interface ISourceMock : ISource
{
}

public interface IDosageMock : IDosage
{
    Task NoSourceAsync(IInterfaceLogger logger);
}

public abstract class SourceMock(ILogger<SourceMock> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator) : ISourceMock, IDosageMock
{
    protected bool UseLoadpoint = true;

    protected ILogger<SourceMock>? _logger = logger;
    protected SourceCapabilities _sourceCapabilities = sourceCapabilities;
    protected LoadpointInfo _info = new();
    protected DosageProgress _status = new();
    protected DateTime _startTime;
    protected ActiveEnergy _dosageEnergy;
    protected bool _dosageMode = false;
    protected TargetLoadpoint? _loadpoint;
    protected ISourceCapabilityValidator _validator = validator;

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        CorrectFrequencyValue(loadpoint);

        var isValid = _validator.IsValid(loadpoint, _sourceCapabilities);

        if (isValid == SourceApiErrorCodes.SUCCESS)
        {
            _logger?.LogTrace("Loadpoint set, source turned on.");
            _loadpoint = loadpoint;

            _info.SavedAt = _info.ActivatedAt = DateTime.Now;
            _info.IsActive = true;
        }

        _dosageMode = false;

        return Task.FromResult(isValid);
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        _logger?.LogTrace("Source turned off.");

        _info.IsActive = false;

        return Task.FromResult(SourceApiErrorCodes.SUCCESS);
    }

    public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_sourceCapabilities);

    public Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
    {
        _dosageMode = on;

        return Task.CompletedTask;
    }

    public Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        _dosageEnergy = value;

        return Task.CompletedTask;
    }

    public Task StartDosageAsync(IInterfaceLogger logger)
    {
        _startTime = DateTime.Now;
        _status.Active = true;
        _dosageMode = false;

        return Task.CompletedTask;
    }

    public Task CancelDosageAsync(IInterfaceLogger logger)
    {
        _status.Active = false;
        _status.Remaining = ActiveEnergy.Zero;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="meterConstant"></param>
    /// <returns></returns>
    public abstract Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant);

    public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
    {
        _logger?.LogTrace("Mock switches off the current for dosage");

        return Task.FromResult(_dosageMode);
    }

    public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_info);

    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected ActiveEnergy DosageDone()
    {
        _status.Active = false;

        _dosageMode = true;

        return _dosageEnergy;
    }

    public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger logger) => Task.FromResult(_loadpoint);

    public virtual Task NoSourceAsync(IInterfaceLogger logger)
    {
        UseLoadpoint = false;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StartEnergyAsync(IInterfaceLogger logger)
    {
        if (_status.Active) throw new InvalidOperationException("dosage active");

        _startTime = DateTime.Now;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopEnergyAsync(IInterfaceLogger logger) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger) => throw new NotImplementedException();

    private static void CorrectFrequencyValue(TargetLoadpoint loadPoint)
    {
        if (loadPoint.Frequency.Mode == FrequencyMode.GRID_SYNCHRONOUS)
            loadPoint.Frequency.Value = new Frequency(50d);
        else if (loadPoint.Frequency.Mode == FrequencyMode.THIRD_OF_GRID_SYNCHRONOUS)
            loadPoint.Frequency.Value = new Frequency(50d / 3);
    }
}

