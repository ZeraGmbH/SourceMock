namespace ErrorCalculatorApi.Exceptions;

/// <summary>
/// Generated when a frequency generator connected error calculator
/// should be used prior to configuring the meter test system itself.
/// </summary>
public class ErrorCalculatorNotReadyException() : Exception("Error calculator must be configured before it can be used")
{
}
