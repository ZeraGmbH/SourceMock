using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.Models;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// 
/// </summary>
public class DatabaseErrorFilter : IExceptionFilter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is ItemNotFoundException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(context.Exception.Message, status: 422, samErrorCode: SamDatabaseError.ITEM_NOT_FOUND, "");
            context.ExceptionHandled = true;
            return;
        }

        context.Result = ErrorHelper.CreateProblemDetails(context.Exception.Message, status: 400, samErrorCode: SamDatabaseError.DATABASE_ERROR, "");
        context.ExceptionHandled = true;
    }

}
