using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using MeterTestSystemApi.Models.ConfigurationProviders;
using ZERA.WebSam.Shared.Security;
using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Controllers;

/// <summary>
/// Controller to probe the configuration.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ConfigurationProbingController(IProbeConfigurationService prober, IServiceProvider services) : ControllerBase
{
    /// <summary>
    /// Start a new probe.
    /// </summary>
    /// <param name="request">Probe configuration to use.</param>
    /// <param name="dryRun">Set to only get a list of what will be probed.</param>
    [HttpPost, SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "StartConfigurationProbe")]
    public async Task<ActionResult> StartAsync([FromBody] ProbeConfigurationRequest request, bool dryRun = false)
    {
        await prober.StartProbeAsync(request, dryRun, services);

        return Ok();
    }

    /// <summary>
    /// Manually executes probes.
    /// </summary>
    /// <param name="probes">List of probes.</param>
    /// <returns>Result from the probes in the order of requests.</returns>
    [HttpPost("probe"), SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "ManualProbe")]
    public string[] Probe([FromBody] List<Probe> probes)
    {
        return [.. probes.Select(p => $"{p} not yet implemented")];
    }

    /// <summary>
    /// Cancel the current probing.
    /// </summary>
    [HttpDelete, SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "CancelConfigurationProbe")]
    public async Task<ActionResult> CancelAsync()
    {
        await prober.AbortAsync();

        return Ok();
    }

    /// <summary>
    /// Get the result of the last probing.
    /// </summary>
    /// <returns>null if a probing is still active.</returns>
    [HttpGet, SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "GetConfigurationProbeResult")]
    public ActionResult<ProbeConfigurationResult?> GetResult()
        => prober.IsActive ? NoContent() : prober.Result;
}
