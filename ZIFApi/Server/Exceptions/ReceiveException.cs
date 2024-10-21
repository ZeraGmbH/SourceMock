namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class ReceiveException(Exception inner) : Exception($"Unable to receive: {inner.Message}", inner)
{
}