using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ZERA.WebSam.Shared.ExceptionHandling;

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
            context.Result = ErrorHelper.CreateProblemDetails(
                    exception.Message,
                    status: StatusCodes.Status499ClientClosedRequest,
                    samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_NOT_READY
                );

        else if (exception is CommandFailedException)
            context.Result = ErrorHelper.CreateProblemDetails(
                    exception.Message,
                    status: StatusCodes.Status422UnprocessableEntity,
                    samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_COMMAND_FAILED
                );

        else if (exception is JobFailedException)
            context.Result = ErrorHelper.CreateProblemDetails(
                    exception.Message,
                    status: StatusCodes.Status406NotAcceptable,
                    samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_JOB_FAILED
                );

        else if (exception is NotConnectedException)
            context.Result = ErrorHelper.CreateProblemDetails(
                    exception.Message,
                    status: StatusCodes.Status408RequestTimeout,
                    samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_NOT_CONNECTED
                );

        else if (exception is UnableToSendException)
            context.Result = ErrorHelper.CreateProblemDetails(
                    exception.Message,
                    status: StatusCodes.Status410Gone,
                    samErrorCode: ErrorCalculatorApiErrorCodes.ERROR_CALCULATOR_SEND_FAILED
                );

        else return;

        context.ExceptionHandled = true;
    }

}
