using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// Creates ProblemDetails that are extended with 
/// SamDetailsExtension including an ErrorCode and additional arguments
/// </summary>
public static class ErrorHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Some enum</typeparam>
    /// <param name="detail">e.g. message from exception</param>
    /// <param name="status">HTTP status code</param>
    /// <param name="samErrorCode">e.g. SamDatabaseError.ITEM_NOT_FOUND</param>
    /// <param name="args">some arguments, e.g. itemID</param>
    /// <returns></returns>
    public static ActionResult CreateProblemDetails<T>(string detail, int status, T samErrorCode, params object[]? args) where T : notnull
    {
        return new ObjectResult(new ProblemDetails
        {
            Detail = detail,
            Status = status,
            Extensions = {
                { SamDetailExtensions.SamErrorCode.ToString(), samErrorCode.ToString() },
                { SamDetailExtensions.SamErrorArgs.ToString(), args }
             }
        });
    }
}