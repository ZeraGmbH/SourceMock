using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using SharedLibrary.Models.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace ErrorCalculatorApi.Controllers;

/// <summary>
/// Controller to manage error calculators.
/// </summary>
/// <remarks>
/// Initialize a new controller.
/// </remarks>
/// <param name="devices">Connected device to use.</param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ErrorCalculatorController(IErrorCalculator[] devices, IInterfaceLogger interfaceLogger) : ControllerBase
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
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].SetErrorMeasurementParameters(interfaceLogger, new(dutMeterConstant), new(impulses), new(refMeterMeterConstant)));

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
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].GetFirmwareVersion(interfaceLogger));

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
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].GetErrorStatus(interfaceLogger));

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
    public Task<ActionResult> StartSingle(ErrorCalculatorMeterConnections? connection, int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].StartErrorMeasurement(interfaceLogger, false, connection));

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
    public Task<ActionResult> StartContinuous(ErrorCalculatorMeterConnections? connection, int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].StartErrorMeasurement(interfaceLogger, true, connection));

    /// <summary>
    /// Get all supported physical connections.
    /// </summary>
    [HttpGet("GetSupportedMeterConnections/{pos?}")]
    [SwaggerOperation(OperationId = "GetSupportedMeterConnections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ErrorCalculatorMeterConnections[]>> GetSupportedMeterConnections(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(devices[pos].GetSupportedMeterConnections);

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
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].AbortErrorMeasurement(interfaceLogger));

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
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].AbortAllJobs(interfaceLogger));

    /// <summary>
    /// Make sure that source is connected to device under test.
    /// </summary>
    [HttpPost("ActivateSource/{pos?}")]
    [SwaggerOperation(OperationId = "ActivateSource")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> ActivateSource(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].ActivateSource(interfaceLogger, true));

    /// <summary>
    /// Disconnect source from all devices under test.
    /// </summary>
    [HttpPost("DeactivateSource/{pos?}")]
    [SwaggerOperation(OperationId = "DeactivateSource")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> DeactivateSource(int pos = 0) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => devices[pos].ActivateSource(interfaceLogger, false));

    /// <summary>
    /// Report if the error calculator can be used.
    /// </summary>
    [HttpGet("Available/{pos?}")]
    [SwaggerOperation(OperationId = "ErrorCalculatorIsAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> IsAvailable(int pos = 0) =>
        Ok(devices[pos].GetAvailable(interfaceLogger));

    /// <summary>
    /// Report the number of error calculators.
    /// </summary>
    [HttpGet("Count")]
    [SwaggerOperation(OperationId = "NumberOfErrorCalculators")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<int> Count() =>
        Ok(devices.Length);
}
