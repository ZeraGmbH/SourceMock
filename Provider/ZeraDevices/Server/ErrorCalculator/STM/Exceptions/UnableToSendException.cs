namespace ZeraDevices.ErrorCalculator.STM.Exceptions;

/// <summary>
/// Failed when writing data to the error calculator.
/// </summary>
public class UnableToSendException(Exception inner) : Exception("unable to send data to error calculator", inner)
{
}
