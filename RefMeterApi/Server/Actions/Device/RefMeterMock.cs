using RefMeterApi.Models;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class RefMeterMock : IRefMeter
{

    /// <summary>
    /// 
    /// </summary>
    public bool Available => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<MeasureOutput> GetActualValues()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        throw new NotImplementedException();
    }

}
