namespace ZIFApi.Models;

/// <summary>
/// 
/// </summary>
public interface IZIFDevicesFactory
{
    /// <summary>
    /// 
    /// </summary>
    void Initialize(IEnumerable<ZIFConfiguration> sockets);

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    IZIFDevice[] Devices { get; }
}