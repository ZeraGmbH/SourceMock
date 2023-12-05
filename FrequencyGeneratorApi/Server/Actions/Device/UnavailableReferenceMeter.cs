using RefMeterApi.Actions.Device;
using RefMeterApi.Models;

namespace MeteringSystemApi;

internal class UnavailableReferenceMeter : IRefMeter
{
    public bool Available => false;

    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        throw new NotImplementedException();
    }

    public Task<MeasureOutput> GetActualValues()
    {
        throw new NotImplementedException();
    }

    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        throw new NotImplementedException();
    }

    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        throw new NotImplementedException();
    }
}
