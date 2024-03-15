namespace ErrorCalculatorApi.Exceptions;

/// <summary>
/// Error calculator job reports an error.
/// </summary>
public class JobFailedException() : Exception("error calculator job failed")
{
}
