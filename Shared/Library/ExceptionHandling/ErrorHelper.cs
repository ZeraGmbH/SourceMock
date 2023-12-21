using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// ProblemDetails object
/// </summary>
public static class ErrorHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="detail"></param>
    /// <param name="status"></param>
    /// <param name="samErrorCode"></param>
    /// <param name="args"></param>
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