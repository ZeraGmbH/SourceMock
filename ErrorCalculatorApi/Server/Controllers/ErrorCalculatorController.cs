using System.ComponentModel.DataAnnotations;
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
/// <param name="devices">Connected device to use.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ErrorCalculatorController(IErrorCalculator[] devices) : ControllerBase
{
    /// <summary>
    /// Configure the error measurement.
    /// </summary>
    /// <param name="dutMeterConstant">Meter constant for the DUT - impulses per kWh.</param>
    /// <param name="impulses">Number of impulses.</param>
    /// <param name="refMeterMeterConstant">Meter constant for the reference meter - impulses per kWh.</param>
    /// <param name="pos"></param>
    [HttpPut("{pos?}")]
    [SwaggerOperation(OperationId = "SetParameters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetParameters(double dutMeterConstant, long impulses, double refMeterMeterConstant, int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].SetErrorMeasurementParameters(dutMeterConstant, impulses, refMeterMeterConstant));

    /// <summary>
    /// Retrieve the current firmware of the error calculator.
    /// </summary>
    /// <returns>Firmware information.</returns>
    [HttpGet("Version/{pos?}")]
    [SwaggerOperation(OperationId = "GetFirmware")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ErrorCalculatorFirmwareVersion>> GetFirmware(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].GetFirmwareVersion);

    /// <summary>
    /// Retrieve the current status of the error measurement.
    /// </summary>
    /// <returns>Status information.</returns>
    [HttpGet("{pos?}")]
    [SwaggerOperation(OperationId = "GetStatus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatus(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].GetErrorStatus);

    /// <summary>
    /// Start a single error measurement.
    /// </summary>
    [HttpPost("StartSingle/{pos?}")]
    [SwaggerOperation(OperationId = "StartSingle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> StartSingle(ErrorCalculatorConnections? connection, int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].StartErrorMeasurement(false, connection));

    /// <summary>
    /// Start a continous error measurement.
    /// </summary>
    [HttpPost("StartContinuous/{pos?}")]
    [SwaggerOperation(OperationId = "StartContinuous")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> StartContinuous(ErrorCalculatorConnections? connection, int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].StartErrorMeasurement(true, connection));

    /// <summary>
    /// Get all supported physical connections.
    /// </summary>
    [HttpGet("SupportedConnections/{pos?}")]
    [SwaggerOperation(OperationId = "GetSupportedConnections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ErrorCalculatorConnections[]>> GetSupportedConnections(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].GetSupportedConnections);

    /// <summary>
    /// Abort the error measurement.
    /// </summary>
    [HttpPost("Abort/{pos?}")]
    [SwaggerOperation(OperationId = "Abort")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> Abort(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].AbortErrorMeasurement);

    /// <summary>
    /// Abort the error measurement.
    /// </summary>
    [HttpPost("AbortAll/{pos?}")]
    [SwaggerOperation(OperationId = "AbortAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> AbortAll(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].AbortAllJobs);

    /// <summary>
    /// Report if the error calculator can be used.
    /// </summary>
    [HttpGet("Available/{pos?}")]
    [SwaggerOperation(OperationId = "ErrorCalculatorIsAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> IsAvailable(int pos = 0) =>
        Ok(devices[pos].Available);

    /// <summary>
    /// Report the number of error calculators.
    /// </summary>
    [HttpGet("Count")]
    [SwaggerOperation(OperationId = "NumberOfErrorCalculators")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<int> Count() =>
        Ok(devices.Length);
}
