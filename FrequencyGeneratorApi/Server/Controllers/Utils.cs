using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeteringSystemApi.Controllers;

class Utils
{
    public static async Task<ActionResult<T>> SafeExecuteSerialPortCommand<T>(Func<Task<T>> method) =>
        (ActionResult<T>)await DoSafeExecuteSerialPortCommand(async () => new OkObjectResult(await method()));

    public static Task<ActionResult> SafeExecuteSerialPortCommand(Func<Task> method) =>
        DoSafeExecuteSerialPortCommand(async () =>
        {
            await method();

            return new OkResult();
        });

    private static async Task<ActionResult> DoSafeExecuteSerialPortCommand(Func<Task<ActionResult>> method)
    {

        try
        {
            return await method();
        }
        catch (TimeoutException)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = "Source operation timed out.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (InvalidOperationException e)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Unable to execute request: {e.Message}.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (ArgumentException e)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Unable to execute request: {e.Message}.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (OperationCanceledException e)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Execution has been cancelled: {e.Message}.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (Exception e)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = $"Operation failed: {e.Message}.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}