using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RefMeterApi.Controllers;

/// <summary>
/// Request device dependant information.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class RefMeterController : ControllerBase
{
    private readonly IRefMeterDevice _device;

    /// <summary>
    /// Initialize a new controller.
    /// </summary>
    /// <param name="device">Serial port connected device to use.</param>
    public RefMeterController(IRefMeterDevice device)
    {
        _device = device;
    }

    /// <summary>
    /// Get the current measurement data.
    /// </summary>
    /// <returns>The current data.</returns>
    [HttpGet("CurrentMeasure")]
    [SwaggerOperation(OperationId = "GetCurrentMeasure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasureOutput>> GetCurrentMeasureOutput() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetActualValues);

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("MeasurementModes")]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModes() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetMeasurementModes);

    /// <summary>
    /// Get the current measurement mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    [HttpGet("MeasurementMode")]
    [SwaggerOperation(OperationId = "GetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementMode() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetActualMeasurementMode);

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode")]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetActualMeasurementMode(MeasurementModes mode) =>
        Utils.SafeExecuteSerialPortCommand(() => _device.SetActualMeasurementMode(mode));
}
