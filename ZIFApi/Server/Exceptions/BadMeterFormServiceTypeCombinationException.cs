using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class BadMeterFormServiceTypeCombinationException(string meterForm, string serviceType)
    : ZIFException($"Unsupported pair of meter form '{meterForm}' and service type: '{serviceType}'")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.BadMeterServicePair;
}