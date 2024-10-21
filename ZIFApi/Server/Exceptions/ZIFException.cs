using ZIFApi.Models;

namespace ZIFApi.Exceptions;

/// <summary>
/// 
/// </summary>
public abstract class ZIFException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public abstract ZIFErrorCodes ErrorCode { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public ZIFException(string message) : base(message) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    /// <returns></returns>
    public ZIFException(string message, Exception inner) : base(message, inner) { }
}