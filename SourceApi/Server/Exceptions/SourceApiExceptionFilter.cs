using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.ExceptionHandling;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Exceptions;

/// <summary>
/// Handles all exceptions that are thrown by the SourceApi
/// </summary>
public class SourceApiExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is SourceNotReadyException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(
                detail: exception.Message,
                status: StatusCodes.Status400BadRequest,
                samErrorCode: SourceApiErrorCodes.SOURCE_NOT_READY
            );
            context.ExceptionHandled = true;
            return;
        }
    }

}
