using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using MeterTestSystemApi.Models.ConfigurationProviders;
using ZERA.WebSam.Shared.Security;
using MeterTestSystemApi.Services;
using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Controllers;

/// <summary>
/// Controller to probe the configuration.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ConfigurationProbingController(IProbeConfigurationService prober, IProbingOperationStore operations, IServiceProvider services) : ControllerBase
{
    /// <summary>
    /// Start a new probe.
    /// </summary>
    /// <param name="request">Probe configuration to use.</param>
    [HttpPost, SamAuthorize(WebSamRole.testsystemadmin)]
    [SwaggerOperation(OperationId = "StartConfigurationProbe")]
    public async Task<ActionResult> StartAsync([FromBody] ProbeConfigurationRequest request)
    {
        await prober.ConfigureProbingAsync(request, services);

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
    [SwaggerOperation(OperationId = "GetLatestConfigurationProbe")]
    public async Task<ActionResult<ProbingOperation?>> GetLatestAsync()
    {
        var latest = await Task.Run(() => operations.Query().OrderByDescending(o => o.Created).FirstOrDefault());

        if (latest != null) return Ok(latest);

        return NoContent();
    }
}
