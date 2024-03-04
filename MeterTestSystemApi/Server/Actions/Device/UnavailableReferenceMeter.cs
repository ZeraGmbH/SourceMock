using RefMeterApi.Actions.Device;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of a reference meter not yet configured.
/// </summary>
internal class UnavailableReferenceMeter : IRefMeter
{
    public bool Available => false;

    public Task<MeasurementModes?> GetActualMeasurementMode() => throw new RefMeterNotReadyException();

    public Task<MeasuredLoadpoint> GetActualValues(int firstActiveVoltagePhase = -1) => throw new RefMeterNotReadyException();

    public Task<MeasurementModes[]> GetMeasurementModes() => throw new RefMeterNotReadyException();

    public Task SetActualMeasurementMode(MeasurementModes mode) => throw new RefMeterNotReadyException();
}
