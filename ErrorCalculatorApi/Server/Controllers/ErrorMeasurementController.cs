using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;

namespace ErrorCalculatorApi.Controllers;

/// <summary>
/// Controller to manage error calculators.
/// </summary>
/// <remarks>
/// Initialize a new controller.
/// </remarks>
/// <param name="device">Serial port connected device to use.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/ErrorCalculator/[controller]")]
public class ErrorMeasurementController(IErrorCalculator device) : ControllerBase
{
    /// <summary>
    /// The error calculator to use during this request execution.
    /// </summary>
    private readonly IErrorCalculator _device = device;

    /// <summary>
    /// Configure the error measurement.
    /// </summary>
    /// <param name="meterConstant">Meter constant for the DUT - impulses per kWh.</param>
    /// <param name="impulses">Number of impulses.</param>
    [HttpPut]
    [SwaggerOperation(OperationId = "SetParameters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetParameters(double meterConstant, long impulses) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.SetErrorMeasurementParameters(meterConstant, impulses));

    /// <summary>
    /// Retrieve the current status of the error measurement.
    /// </summary>
    /// <returns>Status information.</returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetStatus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatus() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(_device.GetErrorStatus);

    /// <summary>
    /// Start a single error measurement.
    /// </summary>
    [HttpPost("StartSingle")]
    [SwaggerOperation(OperationId = "StartSingle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> StartSingle() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.StartErrorMeasurement(false));

    /// <summary>
    /// Start a coninous error measurement.
    /// </summary>
    [HttpPost("StartContinuous")]
    [SwaggerOperation(OperationId = "StartContinuous")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> StartContinuous() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.StartErrorMeasurement(true));

    /// <summary>
    /// Abort the error measurement.
    /// </summary>
    [HttpPost("Abort")]
    [SwaggerOperation(OperationId = "Abort")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> Abort() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(_device.AbortErrorMeasurement);

    /// <summary>
    /// Report if the error calculator can be used.
    /// </summary>
    [HttpGet("Available")]
    [SwaggerOperation(OperationId = "ErrorCalculatorIsAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> IsAvailable() =>
        Ok(_device.Available);
}
