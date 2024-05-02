using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SharedLibrary.Models;
using SharedLibrary.Models.Logging;

namespace SharedLibrary.Actions;

/// <summary>
/// 
/// </summary>
public class LoggingHttpClient(HttpClient client) : ILoggingHttpClient
{
    /// <summary>
    /// Configure serializer to generate camel casing.
    /// </summary>
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
        Converters = { new StringEnumConverter() }
    };

    /// <inheritdoc/>
    public InterfaceLogEntryConnection LogConnection { get; set; } = null!;

    /// <inheritdoc/>
    public HttpRequestHeaders DefaultRequestHeaders => client.DefaultRequestHeaders;

    /// <summary>
    /// Execute a single HTTP request with logging.
    /// </summary>
    /// <param name="logger">Scoped logging service.</param>
    /// <param name="payload">Payload to log.</param>
    /// <param name="action">Request to execute.</param>
    /// <param name="withLog">Unset to generate log entry by caller.</param>
    /// <returns>Information to log a matching response.</returns>
    private async Task<Tuple<HttpResponseMessage, IPreparedInterfaceLogEntry?, InterfaceLogPayload>> DoRequest(IInterfaceLogger logger, string payload, Func<Task<HttpResponseMessage>> action, bool withLog = true)
    {
        /* Prepare logging. */
        var requestId = Guid.NewGuid().ToString();
        var connection = LogConnection == null ? null : logger.CreateConnection(LogConnection);

        var send = connection?.Prepare(new() { RequestId = requestId, Outgoing = true });

        var sendPayload = new InterfaceLogPayload
        {
            Encoding = InterfaceLogPayloadEncodings.Raw,
            Payload = payload,
            PayloadType = "",
        };

        /* Execute the request. */
        HttpResponseMessage response = null!;

        try
        {
            response = await action();

            /* Check for status. */
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                throw new InvalidOperationException(response.ReasonPhrase);
        }
        catch (Exception e)
        {
            sendPayload.TransferExecption = e.Message;

            /* Caller will handle execption but logging will always be done. */
            throw;
        }
        finally
        {
            /* Always log. */
            send?.Finish(sendPayload);
        }

        /* Prepare logging. */
        var receive = connection?.Prepare(new() { RequestId = requestId, Outgoing = false });

        var receivePayload = new InterfaceLogPayload
        {
            Encoding = InterfaceLogPayloadEncodings.Raw,
            Payload = $"HTTP {HttpStatusCode.NoContent}",
            PayloadType = "",
        };

        /* Do logging if required. */
        if (withLog) receive?.Finish(receivePayload);

        /* Report logging context to reuse in response - if necessary. */
        return Tuple.Create(response, receive, receivePayload)!;
    }

    /// <summary>
    /// Execute a HTTP request and convert the response to JSON.
    /// </summary>
    /// <typeparam name="T">Type of the response.</typeparam>
    /// <param name="logger">Scoped interface logger to use.</param>
    /// <param name="payload">Payload to report.</param>
    /// <param name="action">HTTP request to execute,</param>
    /// <returns>Instance of the requested type.</returns>
    private async Task<T> DoRequest<T>(IInterfaceLogger logger, string payload, Func<Task<HttpResponseMessage>> action)
    {
        /* Execute request. */
        var (response, receive, receivePayload) = await DoRequest(logger, payload, action, false);

        try
        {
            /* No result. */
            if (response.StatusCode == HttpStatusCode.NoContent) return default!;

            /* Get the response. */
            receivePayload.Payload = await response.Content.ReadAsStringAsync();
            receivePayload.Encoding = InterfaceLogPayloadEncodings.Json;
        }
        catch (Exception e)
        {
            receivePayload.TransferExecption = e.Message;

            /* Caller will handle execption but logging will always be done. */
            throw;
        }
        finally
        {
            /* Always log. */
            receive?.Finish(receivePayload);
        }

        /* Convert to json. */
        return JsonConvert.DeserializeObject<T>(receivePayload.Payload)!;
    }

    /// <inheritdoc/>
    public Task<T> GetAsync<T>(IInterfaceLogger interfaceLogger, Uri uri)
        => DoRequest<T>(interfaceLogger, uri.ToString(), () => client.GetAsync(uri));

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(IInterfaceLogger interfaceLogger, Uri uri)
        => (await DoRequest(interfaceLogger, uri.ToString(), () => client.PostAsync(uri, null))).Item1;

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PutAsync(IInterfaceLogger interfaceLogger, Uri uri)
        => (await DoRequest(interfaceLogger, uri.ToString(), () => client.PutAsync(uri, null))).Item1;

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PutAsync<TPayload>(IInterfaceLogger interfaceLogger, Uri uri, TPayload payload)
    {
        var content = JsonConvert.SerializeObject(payload, JsonSettings);

        return (await DoRequest(interfaceLogger, $"{uri}\n\n{content}", () =>
            client.PutAsync(uri, new StringContent(
                content,
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )))).Item1;
    }
}
