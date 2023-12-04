using MeteringSystemApi.Models;
using RefMeterApi.Models;
using SourceApi.Model;

namespace MeteringSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortMTMeteringSystem : IMeteringSystem
{
    /// <inheritdoc/>
    public Task<MeteringSystemCapabilities> GetCapabilities() => Task.FromResult<MeteringSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter)
    {
        throw new NotImplementedException();
    }
}
