namespace ZIFApi.Models;

/// <summary>
/// 
/// </summary>
public interface IZIFDevicesFactory : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    void Initialize(List<ZIFConfiguration> sockets);

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    IZIFDevice[] Devices { get; }
}