using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;
using ZIFApi.Models;

namespace ZIFApi.Controller;

/// <summary>
/// Access all installed ZIF sockets.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ZIFController(IZIFDevice[] devices, IZIFServiceTypeLookup typeLookup, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// Get the number of configured ZIF sockets. This will include
    /// socket positions that are currently offline.
    /// </summary>
    [HttpGet("Count"), SamAuthorize]
    [SwaggerOperation(OperationId = "NumberOfZIFSockets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<int> Count() => Ok(devices.Length);

    /// <summary>
    /// See if a specific ZIF socket is online.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpGet("Available/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "IsZIFSocketAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> Available(int pos = 0) => Ok(devices[pos] != null);

    /// <summary>
    /// Get the firmware version for one socket.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpGet("Version/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetZIFSocketVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<ZIFVersionInfo>> GetVersionAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].GetVersionAsync(interfaceLogger));

    /// <summary>
    /// Get the serial number of the ZIF socket at a specific position.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    /// <returns></returns>
    [HttpGet("SerialNumber/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetZIFSocketSerial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<int>> GetSerialAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].GetSerialAsync(interfaceLogger));

    /// <summary>
    /// See if a ZIF socket has been activated and is ready for testing.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpGet("Active/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "IsZIFSocketActive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<bool>> GetActiveAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].GetActiveAsync(interfaceLogger));

    /// <summary>
    /// See if a meter is installed on a socket - this will report the 
    /// meter state even if the socket has not yet been activated.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpGet("HasMeter/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "HasZIFSocketMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<bool>> GetHasMeterAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].GetHasMeterAsync(interfaceLogger));

    /// <summary>
    /// See a socket is in array - to reset activate or deactivate it
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpGet("HasError/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "HasZIFSocketError")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult<bool>> GetHasErrorAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].GetHasErrorAsync(interfaceLogger));

    /// <summary>
    /// Activate a specific socket so that it can be used for measurements.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpPost("Activate/{pos?}"), SamAuthorize(WebSamRole.testcaseeditor)]
    [SwaggerOperation(OperationId = "ActivateZIFSocket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult> ActivateAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].SetActiveAsync(true, interfaceLogger));

    /// <summary>
    /// Deactivate a specific socket.
    /// </summary>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpPost("Deactivate/{pos?}"), SamAuthorize(WebSamRole.testcaseeditor)]
    [SwaggerOperation(OperationId = "DeactivateZIFSocket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult> DeactivateAsync(int pos = 0)
        => ActionMapper.SafeExecuteAsync(() => devices[pos].SetActiveAsync(false, interfaceLogger));

    /// <summary>
    /// Switch socket to a given meter form and service type.
    /// </summary>
    /// <param name="meterForm">The meterform to use.</param>
    /// <param name="serviceType">The service type to use.</param>
    /// <param name="pos">Zero-based index of the socket.</param>
    [HttpPost("{pos?}"), SamAuthorize(WebSamRole.testcaseeditor)]
    [SwaggerOperation(OperationId = "SetMeterFormAndServiceType")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SetMeterAsync(string meterForm, string? serviceType, int pos = 0)
    {
        // May auto-detect service type.
        if (string.IsNullOrEmpty(serviceType))
        {
            var serviceTypes = await typeLookup.GetServiceTypesOfMeterForm(meterForm);

            serviceType = serviceTypes.Single();
        }

        return await ActionMapper.SafeExecuteAsync(() => devices[pos].SetMeterAsync(meterForm, serviceType, interfaceLogger));
    }
}