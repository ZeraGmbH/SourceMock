using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using RefMeterApi.Models;
using SharedLibrary.ExceptionHandling;

namespace RefMeterApi.Exceptions;

/// <summary>
/// Handles all specific exceptions that are thrown in the RefMeterApi
/// </summary>
public class RefMeterApiExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Is executed if an exception is thrown
    /// </summary>
    /// <param name="context">ExceptionContext is given by the MVC pipeline</param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is RefMeterNotReadyException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(
                exception.Message,
                status: StatusCodes.Status400BadRequest,
                samErrorCode: RefMeterApiErrorCodes.REF_METER_NOT_READY
                );
            context.ExceptionHandled = true;
            return;
        }
    }

}
