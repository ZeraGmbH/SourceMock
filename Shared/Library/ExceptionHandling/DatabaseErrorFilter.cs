using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
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
        if (exception is ItemNotFoundException itemNotFoundException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(exception.Message, status: 422, samErrorCode: SamDatabaseError.ITEM_NOT_FOUND, itemNotFoundException.ItemId);
            context.ExceptionHandled = true;
            return;
        }
        if (exception is MongoException)
        {
            context.Result = ErrorHelper.CreateProblemDetails(exception.Message, status: 400, samErrorCode: SamDatabaseError.DATABASE_ERROR, "");
            context.ExceptionHandled = true;
        }
    }

}
