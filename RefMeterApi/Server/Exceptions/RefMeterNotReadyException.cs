namespace RefMeterApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class RefMeterNotReadyException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public RefMeterNotReadyException() : base("Reference Meter must be configured before it can be used") { }
}
