using ErrorCalculatorApi.Actions.Device;
using MeteringSystemApi.Model;
using MeteringSystemApi.Models;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.Source;

namespace MeteringSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface IMeterTestSystem
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
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeters settings);

    /// <summary>
    /// 
    /// </summary>
    AmplifiersAndReferenceMeters AmplifiersAndReferenceMeters { get; }

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
