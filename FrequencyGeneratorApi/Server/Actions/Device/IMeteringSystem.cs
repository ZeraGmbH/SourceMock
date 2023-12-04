using ErrorCalculatorApi.Actions.Device;
using MeteringSystemApi.Model;
using MeteringSystemApi.Models;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeteringSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface IMeteringSystem
{
    /// <summary>
    /// Retrieve information on the firmware version.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<MeteringSystemFirmwareVersion> GetFirmwareVersion();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<MeteringSystemCapabilities> GetCapabilities();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="referenceMeter"></param>
    /// <returns></returns>
    Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter);

    /// <summary>
    /// The corresponding source.
    /// </summary>
    ISource Source { get; }

    /// <summary>
    /// The related reference meter.
    /// </summary>
    IRefMeter RefMeter { get; }

    /// <summary>
    /// The error calculator associated with this metering system.
    /// </summary>
    IErrorCalculator ErrorCalculator { get; }
}
