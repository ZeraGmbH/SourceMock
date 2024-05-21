using System.Net.Http.Headers;
using SharedLibrary.Models.Logging;

namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface ILoggingHttpClient
{
    /// <summary>
    /// 
    /// </summary>
    InterfaceLogEntryConnection LogConnection { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interfaceLogger"></param>
    /// <param name="uri"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> GetAsync<T>(IInterfaceLogger interfaceLogger, Uri uri);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interfaceLogger"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PostAsync(IInterfaceLogger interfaceLogger, Uri uri);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <param name="interfaceLogger"></param>
    /// <param name="uri"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PostAsync<TPayload>(IInterfaceLogger interfaceLogger, Uri uri, TPayload payload);

    /// <summary>
    /// 
    /// </summary>
    HttpRequestHeaders DefaultRequestHeaders { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interfaceLogger"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PutAsync(IInterfaceLogger interfaceLogger, Uri uri);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <param name="interfaceLogger"></param>
    /// <param name="uri"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PutAsync<TPayload>(IInterfaceLogger interfaceLogger, Uri uri, TPayload payload);
}
