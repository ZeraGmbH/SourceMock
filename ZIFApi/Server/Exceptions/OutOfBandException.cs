using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class OutOfBandException(byte got, byte expected) : ZIFException($"Out-Of-Band reply, got 0x{got:x} but expected 0x{expected:x}")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.OutOfBand;
}