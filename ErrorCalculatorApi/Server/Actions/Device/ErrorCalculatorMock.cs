using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class ErrorCalculatorMock : IErrorCalculator
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
    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meterConstant"></param>
    /// <param name="impulses"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="continuous"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task StartErrorMeasurement(bool continuous)
    {
        throw new NotImplementedException();
    }

}
