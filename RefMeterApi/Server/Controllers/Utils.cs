using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RefMeterApi.Controllers;

class Utils
{
    public static async Task<ActionResult<T>> SafeExecuteSerialPortCommand<T>(Func<Task<T>> method)
    {
        {
            try
            {
                return new OkObjectResult(await method());
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
            catch (OperationCanceledException e)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Detail = $"Execution has been cancelled: {e.Message}.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}