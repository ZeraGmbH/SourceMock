using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZERA.WebSam.Shared.ExceptionHandling;
using ZIFApi.Exceptions;

namespace ZIFApi.Controller;

/// <summary>
/// 
/// </summary>
public static class ActionMapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<ActionResult<T>> SafeExecuteAsync<T>(Func<Task<T>> method) =>
          (ActionResult<T>)await DoSafeExecuteAsync(async () => new OkObjectResult(await method()));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Task<ActionResult> SafeExecuteAsync(Func<Task> method) =>
        DoSafeExecuteAsync(async () => { await method(); return new OkResult(); });

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    private static async Task<ActionResult> DoSafeExecuteAsync(Func<Task<ActionResult>> method)
    {
        try
        {
            return await method();
        }
        catch (ZIFException e)
        {
            return ErrorHelper.CreateProblemDetails(
                e.Message,
                StatusCodes.Status400BadRequest,
                e.ErrorCode
            );
        }
    }
}