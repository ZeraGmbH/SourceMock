using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using MeterTestSystemApi.Models.ConfigurationProviders;
using ZERA.WebSam.Shared.Security;
using MeterTestSystemApi.Services;
using MeterTestSystemApi.Actions.Probing;

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
        await prober.ConfigureProbingAsync(request, dryRun, services);

        return Ok();
    }

    /// <summary>
    /// Manually executes probes.
    /// </summary>
    /// <param name="probes">List of probes.</param>
    /// <returns>Result from the probes in the order of requests.</returns>
    [HttpPost("probe"), SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "ManualProbe")]
    public async Task<ActionResult<ProbeInfo[]>> Probe([FromBody] List<Probe> probes)
        => Ok(await prober.ProbeManualAsync(probes, services));

    /// <summary>
    /// Cancel the current probing.
    /// </summary>
    [HttpDelete, SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "CancelConfigurationProbe")]
    public ActionResult Cancel()
    {
        prober.Abort();

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


    /// <summary>
    /// See if a probing operation is active.
    /// </summary>
    /// <returns>Set if a probing operation is active.</returns>
    [HttpGet("active"), SamAuthorize]
    [SwaggerOperation(OperationId = "ConfigurationProbingActive")]
    public ActionResult<bool> TestActive() => Ok(prober.IsActive);
}
