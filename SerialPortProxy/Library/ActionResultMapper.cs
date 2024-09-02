using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZERA.WebSam.Shared.ExceptionHandling;

namespace SerialPortProxy;

/// <summary>
/// Helper mapping serial port exceptions to ActionResults.
/// </summary>
public class ActionResultMapper
{
    /// <summary>
    /// Execute a serial port command which reports a result.
    /// </summary>
    /// <typeparam name="T">Type of the result.</typeparam>
    /// <param name="method">Method to call.</param>
    /// <returns>Corresponding ActionResult as a task.</returns>
    public static async Task<ActionResult<T>> SafeExecuteSerialPortCommandAsync<T>(Func<Task<T>> method) =>
        (ActionResult<T>)await DoSafeExecuteSerialPortCommandAsync(async () => new OkObjectResult(await method()));

    /// <summary>
    /// Exceute a serial port command which produces no result.
    /// </summary>
    /// <param name="method">Method to call.</param>
    /// <returns>ActionResult as a task.</returns>
    public static Task<ActionResult> SafeExecuteSerialPortCommandAsync(Func<Task> method) =>
        DoSafeExecuteSerialPortCommandAsync(async () =>
        {
            /* Execute the command and report success. */
            await method();

            return new OkResult();
        });

    /// <summary>
    /// Execute a serial port command and translate well known exceptions as ActionResult.
    /// </summary>
    /// <param name="method">Method to call.</param>
    /// <returns>ActionResult as a task.</returns>
    private static async Task<ActionResult> DoSafeExecuteSerialPortCommandAsync(Func<Task<ActionResult>> method)
    {
        try
        {
            /* Execute the method and report success. */
            return await method();
        }
        catch (TimeoutException)
        {
            /* No reply in the configured response time. */
            return ErrorHelper.CreateProblemDetails(
                "Source operation timed out.",
                StatusCodes.Status408RequestTimeout,
                SerialPortErrorCodes.SerialPortTimeOut
            );
        }
        catch (InvalidOperationException e)
        {
            /* Bad command. */
            return ErrorHelper.CreateProblemDetails(
                $"Unable to execute request: {e.Message}.",
                StatusCodes.Status406NotAcceptable,
                SerialPortErrorCodes.SerialPortBadRequest,
                e.Message
            );
        }
        catch (ArgumentException e)
        {
            return ErrorHelper.CreateProblemDetails(
                $"Unable to execute request: {e.Message}.",
                StatusCodes.Status400BadRequest,
                SerialPortErrorCodes.SerialPortBadRequest,
                e.Message
            );
        }
        catch (OperationCanceledException e)
        {
            /* Command not executed because it has been canceled before it could be started. */
            return ErrorHelper.CreateProblemDetails(
                $"Execution has been cancelled: {e.Message}.",
                StatusCodes.Status410Gone,
                SerialPortErrorCodes.SerialPortAborted,
                e.Message
            );
        }
    }
}
