using RefMeterApi.Actions.Device;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions;

/// <summary>
/// Implementation of a reference meter not yet configured.
/// </summary>
public class UnavailableReferenceMeter : IRefMeter
{
    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(false);

    /// <inheritdoc/>
    public Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveVoltagePhase = -1) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<Voltage[]> GetVoltageRangesAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<Current[]> GetCurrentRangesAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetVoltageRangeAsync(Voltage voltage)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetCurrentRangeAsync(Current current)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetAutomaticAsync(bool voltageRanges = true, bool currentRanges = true, bool pll = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SelectPllChannelAsync(PllChannel pll)
    {
        throw new NotImplementedException();
    }
}