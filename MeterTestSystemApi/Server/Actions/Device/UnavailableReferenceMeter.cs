using RefMeterApi.Actions.Device;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;
using SharedLibrary.Models.Logging;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of a reference meter not yet configured.
/// </summary>
internal class UnavailableReferenceMeter : IRefMeter
{
    public bool Available => false;

    public Task<double> GetMeterConstant(IInterfaceLogger logger) => throw new NotImplementedException();

    public Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    public Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveVoltagePhase = -1) => throw new RefMeterNotReadyException();

    public Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger) => throw new RefMeterNotReadyException();

    public Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode) => throw new RefMeterNotReadyException();
}
