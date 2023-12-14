namespace ErrorCalculatorApi.Exceptions;

/// <summary>
/// 
/// </summary>
public class ErrorCalculatorNotReadyException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public ErrorCalculatorNotReadyException() : base("Error calculator must be configured before it can be used") { }
}
