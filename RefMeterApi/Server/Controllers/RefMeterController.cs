using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.RefMeter;
using RefMeterApi.Models;
using SerialPortProxy;
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
    private readonly SerialPortConnection _device;

    /// <summary>
    /// Initialize a new controller.
    /// </summary>
    /// <param name="device">Serial port connected device to use.</param>
    public RefMeterController(SerialPortConnection device)
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
    public async Task<ActionResult<MeasureOutput>> GetCurrentMeasureOutput()
    {
        try
        {
            return Ok(await QueryMeasureOutput.Execute(_device));
        }
        catch (TimeoutException)
        {
            return Problem(
                detail: "Source operation timed out.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
        catch (InvalidOperationException e)
        {
            return Problem(
                detail: $"Unable to execute request: {e.Message}.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
        catch (OperationCanceledException e)
        {
            return Problem(
                detail: $"Execution has been cancelled: {e.Message}.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}
