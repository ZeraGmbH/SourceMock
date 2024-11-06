using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Models;

/// <summary>
/// Interface to communicate with a burden.
/// </summary>
public interface IBurden
{
    /// <summary>
    /// Read the version information from the burden.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task<BurdenVersion> GetVersionAsync(IInterfaceLogger interfaceLogger);
}
