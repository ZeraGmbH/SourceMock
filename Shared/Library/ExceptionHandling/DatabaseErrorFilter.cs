using Microsoft.AspNetCore.Mvc.Filters;

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
        throw new NotImplementedException();
    }

}
