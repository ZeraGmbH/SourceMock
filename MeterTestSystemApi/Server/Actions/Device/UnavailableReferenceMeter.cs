using RefMeterApi.Actions.Device;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;

namespace MeterTestSystemApi;

internal class UnavailableReferenceMeter : IRefMeter
{
    public bool Available => false;

    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        throw new RefMeterNotReadyException();
    }

    public Task<MeasureOutput> GetActualValues()
    {
        throw new RefMeterNotReadyException();
    }

    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        throw new RefMeterNotReadyException();
    }

    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        throw new RefMeterNotReadyException();
    }
}
