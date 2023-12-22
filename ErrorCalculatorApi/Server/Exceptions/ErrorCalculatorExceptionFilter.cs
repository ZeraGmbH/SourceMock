using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.ExceptionHandling;

namespace ErrorCalculatorApi.Exceptions;

/// <summary>
/// Handles all exception that are thrown by the ErrorCalculatorApi
/// </summary>
public class ErrorCalculatorExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Is executed if an exception is thrown
    /// </summary>
    /// <param name="context">ExceptionContext is given by the MVC pipeline</param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is ErrorCalculatorNotReadyException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(
                exception.Message,
                status: StatusCodes.Status400BadRequest,
                samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_NOT_READY
                );
            context.ExceptionHandled = true;
            return;
        }
    }

}
