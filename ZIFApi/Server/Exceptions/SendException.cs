namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class SendException(Exception inner) : Exception($"Failed to send: {inner.Message}", inner)
{
}