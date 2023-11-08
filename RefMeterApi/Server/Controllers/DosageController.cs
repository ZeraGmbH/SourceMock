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
public class DosageController : ControllerBase
{
    private readonly IRefMeterDevice _device;

    /// <summary>
    /// Initialize a new controller.
    /// </summary>
    /// <param name="device">Serial port connected device to use.</param>
    public DosageController(IRefMeterDevice device)
    {
        _device = device;
    }

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Start")]
    [SwaggerOperation(OperationId = "Start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> StartDosage() => Utils.SafeExecuteSerialPortCommand(_device.StartDosage);

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Cancel")]
    [SwaggerOperation(OperationId = "Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> CancelDosage() => Utils.SafeExecuteSerialPortCommand(_device.CancelDosage);

    /// <summary>
    /// Change the DOS mode.
    /// </summary>
    /// <param name="on">Set to activate DOS mode - current will be turned on.</param>
    [HttpPost("DOSMode")]
    [SwaggerOperation(OperationId = "SetDOSMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetDOSMode([FromBody] bool on) => Utils.SafeExecuteSerialPortCommand(() => _device.SetDosageMode(on));

    /// <summary>
    /// Read the current progress of a dosage operation.
    /// </summary>
    /// <returns>The information on the current dosage measurement.</returns>
    [HttpGet("Progress")]
    [SwaggerOperation(OperationId = "GetProgress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<DosageProgress>> GetProgress() => Utils.SafeExecuteSerialPortCommand(_device.GetDosageProgress);

    /// <summary>
    /// Set the dosage energy.
    /// </summary>
    /// <param name="energy">The energy to sent to the DUT - in Wh.</param>
    [HttpPut("Energy")]
    [SwaggerOperation(OperationId = "SetEnergy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetEnergy([FromBody] double energy) => Utils.SafeExecuteSerialPortCommand(() => _device.SetDosageEnergy(energy));
}
