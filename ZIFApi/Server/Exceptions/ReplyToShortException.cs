using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class ReplyToShortException() : ZIFException("Not enough data in reply")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.TooShort;
}