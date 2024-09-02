using BarcodeApi.Actions.Device;
using BarcodeApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Security;

namespace ErrorCalculatorApi.Controllers;

/// <summary>
/// Helper to simulate barcode input.
/// </summary>
/// <param name="_device">Serial port connected device to use.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/BarcodeReader/[controller]")]
public class BarcodeReaderController(IBarcodeReader _device) : ControllerBase
{
    /// <summary>
    /// Simulate a barcode.
    /// </summary>
    /// <param name="code">Barcode to use.</param>
    /// <exception cref="NotImplementedException">Barcode reader is not in mock mode.</exception>
    [HttpPut, SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SimulateBarcode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult> SimulateBarcodeAsync([FromBody] string code)
    {
        if (_device is not BarcodeReaderMock mock) throw new NotImplementedException("barcode reader is not a mock");

        mock.Fire(code);

        return Task.FromResult<ActionResult>(Ok());
    }
}
