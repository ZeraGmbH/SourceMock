using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Source connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpClient">Connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestSource(HttpClient httpClient, ILogger<RestSource> logger) : IRestSource
{
    private RestConfiguration _config = null!;

    private Uri _baseUri = null!;

    /// <inheritdoc/>
    public bool Available
    {
        get
        {
            /* Not yet initialized. */
            if (_config == null) return false;

            try
            {
                var available = httpClient.GetAsync(new Uri(_baseUri, "Available")).GetJsonResponse<bool>();

                available.Wait();

                return available.Result;
            }
            catch (Exception e)
            {
                /* Just report the error. */
                logger.LogError("Unable to connect to remote source API: {Exception}",
                    e is AggregateException ae
                    ? ae.InnerExceptions[0].Message
                    : e.Message);

                return false;
            }
        }
    }

    /// <inheritdoc/>
    public Task CancelDosage()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosage()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo()
    {
        var req = httpClient.GetAsync(new Uri(_baseUri, "LoadpointInfo")).GetJsonResponse<LoadpointInfo>();

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities() =>
        httpClient.GetAsync(new Uri(_baseUri, "Capabilities")).GetJsonResponse<SourceCapabilities>();

    /// <inheritdoc/>
    public TargetLoadpoint? GetCurrentLoadpoint()
    {
        var req = httpClient.GetAsync(new Uri(_baseUri, "Loadpoint")).GetJsonResponse<TargetLoadpoint?>();

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Initialize(RestConfiguration endPoint)
    {
        /* Can be only done once. */
        if (_config != null) throw new InvalidOperationException("Already initialized");

        /* Validate.*/
        _baseUri = new Uri(endPoint.EndPoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_baseUri.UserInfo))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_baseUri.UserInfo)));

        /* Did it. */
        _config = Utils.DeepCopy(endPoint);
    }

    /// <inheritdoc/>
    public Task SetDosageEnergy(double value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpoint(TargetLoadpoint loadpoint)
    {
        var res = await httpClient.PutAsJsonAsync(new Uri(_baseUri, "Loadpoint"), loadpoint);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public Task StartDosage()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOff()
    {
        var res = await httpClient.PostAsync(new Uri(_baseUri, "TurnOff"), null);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }
}
