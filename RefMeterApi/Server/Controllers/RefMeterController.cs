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
    /// Get the current measurement data.
    /// </summary>
    /// <returns>The current data.</returns>
    [HttpGet("MeasurementModes")]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModes() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetMeasurementModes);
}
