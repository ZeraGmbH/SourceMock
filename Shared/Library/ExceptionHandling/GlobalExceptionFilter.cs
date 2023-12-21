using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.Models;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// 
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter, IOrderedFilter
{
    /// <summary>
    /// GlobalExceptionFilter will be the last in the mvc pipeline
    /// </summary>
    public int Order => int.MaxValue - 10;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        context.Result = ErrorHelper.CreateProblemDetails(
            exception.Message,
            status: StatusCodes.Status400BadRequest,
            samErrorCode: SamGlobalErrors.GENERAL_ERROR,
            exception.StackTrace ?? ""
        );
        context.ExceptionHandled = true;
    }

}