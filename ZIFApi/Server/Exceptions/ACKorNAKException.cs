namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class ACKorNAKException(byte got) : Exception($"ACK or NAK expected, got 0x{got:x}")
{
}