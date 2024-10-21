namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class OutOfBandException(byte got, byte expected) : Exception($"Out-Of-Band reply, got 0x{got:x} but expected 0x{expected:x}")
{
}