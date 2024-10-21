namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class BadLengthException(int expected) : Exception($"Response should have exactly ${expected} byte(s)")
{
}