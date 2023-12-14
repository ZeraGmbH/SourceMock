using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    public static async Task<ActionResult<T>> SafeExecuteSerialPortCommand<T>(Func<Task<T>> method) =>
        (ActionResult<T>)await DoSafeExecuteSerialPortCommand(async () => new OkObjectResult(await method()));

    /// <summary>
    /// Exceute a serial port command which produces no result.
    /// </summary>
    /// <param name="method">Method to call.</param>
    /// <returns>ActionResult as a task.</returns>
    public static Task<ActionResult> SafeExecuteSerialPortCommand(Func<Task> method) =>
        DoSafeExecuteSerialPortCommand(async () =>
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
    private static async Task<ActionResult> DoSafeExecuteSerialPortCommand(Func<Task<ActionResult>> method)
    {
        try
        {
            /* Execute the method and report success. */
            return await method();
        }
        catch (TimeoutException)
        {
            /* No reply in the configured response time. */
            return new ObjectResult(new ProblemDetails
            {
                Detail = "Source operation timed out.",
                Status = StatusCodes.Status408RequestTimeout
            });
        }
        catch (InvalidOperationException e)
        {
            /* Bad command. */
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Unable to execute request: {e.Message}.",
                Status = StatusCodes.Status406NotAcceptable
            });
        }
        catch (ArgumentException e)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Unable to execute request: {e.Message}.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (OperationCanceledException e)
        {
            /* Command not executed because it has been canceled before it could be started. */
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Execution has been cancelled: {e.Message}.",
                Status = StatusCodes.Status410Gone
            });
        }
        catch (Exception e)
        {
            /* General error while processing the command. */
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Operation failed: {e.Message}.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
