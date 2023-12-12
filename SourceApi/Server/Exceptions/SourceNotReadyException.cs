namespace SourceApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class SourceNotReadyException : Exception
{
    public SourceNotReadyException() : base("Source must be configured before it can be used") { }
}
