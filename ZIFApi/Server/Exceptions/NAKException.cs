using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
/// <param name="response"></param>
/// <returns></returns>
public class NAKException(byte[] response) : ZIFException($"got NACK: {GetNAKReason(response)}")
{
    /// <summary>
    /// 
    /// </summary>
    public override ZIFErrorCodes ErrorCode => ZIFErrorCodes.NAK;

    private static string GetNAKReason(byte[] resonse)
    {
        if (resonse.Length < 1) return "(no details available)";

        switch (resonse[0])
        {
            case 0x01:
                return "serial data error";
            case 0x02:
                return "already communicating with meter";
            case 0x04:
                return "a meter is in the socket";
            case 0x08:
                return "aborted";
            case 0x10:
                return "button depressed";
            case 0x20:
                return "switch error";
            case 0x40:
                return "unknown error";
            default:
                return BitConverter.ToString(resonse);
        }
    }
}