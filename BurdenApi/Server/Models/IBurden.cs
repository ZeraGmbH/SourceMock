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
    /// <returns>Version Information.</returns>
    Task<BurdenVersion> GetVersionAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Ture the burden on or off.
    /// </summary>
    /// <param name="on">Set to turn the burden on.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetActiveAsync(bool on, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Get the names of all burdens.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>List of burdens.</returns>
    Task<string[]> GetBurdensAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Read a single calibration.
    /// </summary>
    /// <param name="burden">Burden to request.</param>
    /// <param name="range">Range to inspect.</param>
    /// <param name="step">Step to request.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>Calibration values or null if not calibrated.</returns>
    Task<Calibration?> GetCalibrationAsync(string burden, string range, string step, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Program burdens.
    /// </summary>
    /// <remarks>This call may take a rather long time.</remarks>
    /// <param name="burden">Burden to program or empty for all burdens.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task ProgramAsync(string? burden, IInterfaceLogger interfaceLogger);
}
