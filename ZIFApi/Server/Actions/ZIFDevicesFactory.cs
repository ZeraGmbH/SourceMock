using ZIFApi.Models;

namespace ZIFApi.Actions;

/// <summary>
/// 
/// </summary>
public class ZIFDevicesFactory : IZIFDevicesFactory
{
    /// <inheritdoc/>
    public IZIFDevice[] Devices { get; private set; } = [];

    /// <inheritdoc/>
    public void Initialize(IEnumerable<ZIFConfiguration> sockets)
    {
    }
}