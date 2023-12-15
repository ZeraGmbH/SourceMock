namespace RefMeterApi.Exceptions;

/// <summary>
/// Indicates that the reference meter used is not yet configured.
/// </summary>
public class RefMeterNotReadyException() : Exception("Reference Meter must be configured before it can be used")
{
}
