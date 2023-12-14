using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;

namespace RefMeterApi.Controllers;

/// <summary>
/// Control a reference meter.
/// </summary>
/// <remarks>
/// Initialize a new controller.
/// </remarks>
/// <param name="device">Serial port connected device to use.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class RefMeterController(IRefMeter device) : ControllerBase
{
    /// <summary>
    /// The current reference meter to use during the request.
    /// </summary>
    private readonly IRefMeter _device = device;

    /// <summary>
    /// Get the current measurement data.
    /// </summary>
    /// <returns>The current data.</returns>
    [HttpGet("ActualValues")]
    [SwaggerOperation(OperationId = "GetActualValues")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasureOutput>> GetActualValues() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(_device.GetActualValues);

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("MeasurementModes")]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModes() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(_device.GetMeasurementModes);

    /// <summary>
    /// Get the current measurement mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    [HttpGet("MeasurementMode")]
    [SwaggerOperation(OperationId = "GetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementMode() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(_device.GetActualMeasurementMode);

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode")]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetActualMeasurementMode(MeasurementModes mode) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.SetActualMeasurementMode(mode));

    /// <summary>
    /// Report if the reference meter can be used.
    /// </summary>
    [HttpGet("Available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "ReferenceMeterIsAvailable")]
    public ActionResult<bool> IsAvailable() =>
        Ok(_device.Available);
}
