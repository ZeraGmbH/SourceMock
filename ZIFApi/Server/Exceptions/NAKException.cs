using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class NAKException() : ZIFException("got NACK")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.NAK;
}