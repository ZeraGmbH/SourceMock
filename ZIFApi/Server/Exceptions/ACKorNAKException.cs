using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class ACKorNAKException(byte got) : ZIFException($"ACK or NAK expected, got 0x{got:x}")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.NotACKorNAK;
}