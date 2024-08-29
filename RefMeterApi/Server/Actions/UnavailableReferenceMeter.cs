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
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => false;

    /// <inheritdoc/>
    public Task<MeterConstant> GetMeterConstant(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveVoltagePhase = -1) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode) => throw new RefMeterNotReadyException();

    /// <inheritdoc/>
    public Task<ReferenceMeterInformation> GetMeterInformation(IInterfaceLogger logger) => throw new RefMeterNotReadyException();
}