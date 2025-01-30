using System.Text.Json;
using ErrorCalculatorApi.Models;
using MeterTestSystemApi.Actions.Probing.HTTP;
using MeterTestSystemApi.Services;
using ZERA.WebSam.Shared;

namespace MeterTestSystemApi.Actions.Probing.KoaLa;

/// <summary>
/// 
/// </summary>
public class STMv1RestProbe(IHttpClientFactory http) : IHttpProbeExecutor
{
    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(HttpProbe probe)
    {
        try
        {
            var endpoint = new Uri(new Uri(probe.EndPoint), "Version/0");

            using var client = http.CreateClient();

            var get = client.GetAsync(endpoint);
            var timeout = Task.Delay(200000);

            var result = await Task.WhenAny(get, timeout);

            if (result != get) throw new TimeoutException("error calculator did not respond in time");

            var response = await get;
            var version = JsonSerializer.Deserialize<ErrorCalculatorFirmwareVersion>(await response.Content.ReadAsStringAsync(), LibUtils.JsonSettings)!;

            return new() { Succeeded = true, Message = $"{version.ModelName} {version.Version}" };
        }
        catch (Exception e)
        {
            return new() { Succeeded = false, Message = e.Message };
        }
    }
}