using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using RefMeterApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RefMeterApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DeviceUnterTestController : ControllerBase
{
    private readonly IDeviceUnderTestStorage _storage;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storage"></param>
    public DeviceUnterTestController(IDeviceUnderTestStorage storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [SwaggerOperation(OperationId = "QueryDevices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<DeviceUnderTest[]> QueryDevices() =>
        Ok(_storage.Query().OrderBy(dut => dut.Name).ToArray());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(OperationId = "AddDevice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceUnderTest>> AddDevice(NewDeviceUnderTest device) =>
        Ok(await _storage.Add(device, User?.Identity?.Name ?? "anonyous"));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="device"></param>
    /// <returns></returns>
    [HttpPut(":id")]
    [SwaggerOperation(OperationId = "UpdateDevice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceUnderTest>> UpdateDevice(string id, NewDeviceUnderTest device) =>
        Ok(await _storage.Update(id, device, User?.Identity?.Name ?? "anonyous"));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete(":id")]
    [SwaggerOperation(OperationId = "DeleteDevice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceUnderTest>> DeleteDevice(string id) =>
        Ok(await _storage.Delete(id, User?.Identity?.Name ?? "anonyous"));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet(":id")]
    [SwaggerOperation(OperationId = "FindDevice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceUnderTest>> FindDevice(string id) =>
        Ok(await _storage.Get(id));
}
