using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// 
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private ProblemDetailsFactory _problemDetailsFactory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="problemDetailsFactory"></param>
    public GlobalExceptionFilter(ProblemDetailsFactory problemDetailsFactory) => _problemDetailsFactory = problemDetailsFactory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var httpContext = context.HttpContext;

        var problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode: 400, detail: exception.Message);
        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
        context.ExceptionHandled = true;
    }

}