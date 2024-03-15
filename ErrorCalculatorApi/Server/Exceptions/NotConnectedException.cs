namespace ErrorCalculatorApi.Exceptions;

/// <summary>
/// Unable to connect to the error calculator.
/// </summary>
public class NotConnectedException() : Exception("no connection to error calculator")
{
}
